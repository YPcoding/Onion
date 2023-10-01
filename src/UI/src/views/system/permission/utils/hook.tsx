import dayjs from "dayjs";
import editForm from "../form.vue";
import { handleTree } from "@/utils/tree";
import { message } from "@/utils/message";
import {
  getPermissionList,
  addPermission,
  updatePermission,
  deletePermission,
  syncAPIToPermission
} from "@/api/system/permission";
import { usePublicHooks } from "../../hooks";
import { addDialog } from "@/components/ReDialog";
import { reactive, ref, onMounted, h } from "vue";
import { type FormItemProps } from "../utils/types";
import { cloneDeep, isAllEmpty } from "@pureadmin/utils";
import { hasAuth, getAuths } from "@/router/utils";

export function usePermission() {
  const form = reactive({
    label: "",
    enabled: null
  });

  const formRef = ref();
  const dataList = ref([]);
  const loading = ref(true);
  const { tagStyle } = usePublicHooks();

  const columns: TableColumnList = [
    {
      label: "权限名称",
      prop: "label",
      width: 180,
      align: "left"
    },
    {
      label: "唯一编码",
      prop: "code",
      minWidth: 300
    },
    {
      label: "类型",
      prop: "type",
      minWidth: 100,
      formatter: row => {
        if (row.type === 10) {
          return "菜单";
        } else if (row.type === 20) {
          return "页面";
        } else if (row.type === 30) {
          return "权限点";
        } else {
          return "";
        }
      }
    },
    {
      label: "描述",
      prop: "description",
      minWidth: 300
    },
    {
      label: "路径",
      prop: "path",
      minWidth: 280
    },
    {
      label: "分组",
      prop: "group",
      minWidth: 180
    },
    {
      label: "请求方法",
      prop: "httpMethods",
      minWidth: 100
    },
    {
      label: "Icon",
      prop: "icon",
      minWidth: 100
    },
    {
      label: "排序",
      prop: "sort",
      minWidth: 50
    },
    {
      label: "状态",
      prop: "enabled",
      minWidth: 60,
      cellRenderer: ({ row, props }) => (
        <el-tag size={props.size} style={tagStyle.value(row.enabled)}>
          {row.enabled === true ? "启用" : "停用"}
        </el-tag>
      )
    },
    {
      label: "创建时间",
      minWidth: 180,
      prop: "created",
      formatter: ({ created }) => dayjs(created).format("YYYY-MM-DD HH:mm:ss")
    },
    {
      label: "操作",
      fixed: "right",
      width: 160,
      slot: "operation",
      hide: () => {
        // 判断权限是否可以显示操作栏
        const auths = ["api:permission:delete", "api:permission:update"];
        return !usePublicHooks().hasAuthIntersection(getAuths(), auths);
      }
    }
  ];

  function handleSelectionChange(val) {
    console.log("handleSelectionChange", val);
  }

  function resetForm(formEl) {
    if (!formEl) return;
    formEl.resetFields();
    onSearch();
  }

  async function onSearch() {
    loading.value = true;
    const { data } = await getPermissionList(); // 这里是返回一维数组结构，前端自行处理成树结构，返回格式要求：唯一id加父节点parentId，parentId取父节点id
    let newData = data;
    if (!isAllEmpty(form.label)) {
      // 前端搜索权限名称
      newData = newData.filter(item => item.label.includes(form.label));
    }
    if (!isAllEmpty(form.enabled)) {
      // 前端搜索状态
      newData = newData.filter(item => item.enabled === form.enabled);
    }
    dataList.value = handleTree(newData); // 处理成树结构
    setTimeout(() => {
      loading.value = false;
    }, 500);
  }

  function formatHigherPermissionOptions(data, superiorId?: string) {
    if (!data || !data.length) return;
    const tree = [];
    for (const item of data) {
      if (item.superiorId === superiorId) {
        const children = formatHigherPermissionOptions(data, item.id);
        if (children.length > 0) {
          item.children = children;
        }
        tree.push(item);
      }
    }
    return tree;
  }

  function openDialog(title = "新增", row?: FormItemProps) {
    addDialog({
      title: `${title}权限`,
      props: {
        formInline: {
          higherPermissionOptions: formatHigherPermissionOptions(
            cloneDeep(dataList.value)
          ),
          sort: row?.sort ?? 0,
          id: row?.id ?? null,
          permissionId: row?.permissionId ?? null,
          superiorId: row?.superiorId ?? null,
          label: row?.label ?? "",
          type: row?.type ?? 10,
          path: row?.path ?? "",
          httpMethods: row?.httpMethods ?? "",
          icon: row?.icon ?? "",
          hidden: row?.hidden ?? true,
          enabled: row?.enabled ?? true,
          closable: row?.closable ?? true,
          opened: row?.opened ?? true,
          newWindow: row?.newWindow ?? true,
          external: row?.external ?? "",
          description: row?.description ?? "",
          group: row?.group ?? "",
          concurrencyStamp: row?.concurrencyStamp ?? null
        }
      },
      width: "40%",
      draggable: true,
      fullscreenIcon: true,
      closeOnClickModal: false,
      contentRenderer: () => h(editForm, { ref: formRef }),
      beforeSure: (done, { options }) => {
        const FormRef = formRef.value.getRef();
        const curData = options.props.formInline as FormItemProps;
        function chores() {
          message(`您${title}了权限名称为${curData.label}的这条数据`, {
            type: "success"
          });
          done(); // 关闭弹框
          onSearch(); // 刷新表格数据
        }
        FormRef.validate(async valid => {
          if (valid) {
            console.log("curData", curData);
            // 表单规则校验通过
            if (title === "新增") {
              await addPermission(curData);
              chores();
            } else {
              await updatePermission(curData);
              chores();
            }
          }
        });
      }
    });
  }

  async function handleDelete(row) {
    await deletePermission({ permissionIds: [row.permissionId] });
    message(`您删除了权限名称为${row.label}的这条数据`, { type: "success" });
    onSearch();
  }

  onMounted(() => {
    onSearch();
  });

  async function handleSyncAPI() {
    var res = await syncAPIToPermission();
    if (res.succeeded) {
      message(`${res.data}`, { type: "success" });
      onSearch();
    } else {
      console.log(res);
      message(`${res.errorMessage}`, { type: "error" });
    }
  }

  return {
    form,
    loading,
    columns,
    dataList,
    onSearch,
    resetForm,
    openDialog,
    handleDelete,
    handleSyncAPI,
    handleSelectionChange
  };
}
