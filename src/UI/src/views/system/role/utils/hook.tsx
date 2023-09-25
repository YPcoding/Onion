import dayjs from "dayjs";
import editForm from "../form.vue";
import menuForm from "../menu.vue";
import { message } from "@/utils/message";
import {
  getRoleList,
  addRole,
  updateRole,
  onbatchDeleteRole,
  getRolePermissionsByRoleId,
  updateRolePermissionMenu
} from "@/api/system/role";
import { ElMessageBox } from "element-plus";
import { usePublicHooks } from "../../hooks";
import { addDialog } from "@/components/ReDialog";
import { type FormItemProps, type MenuFormItemProps } from "../utils/types";
import { type PaginationProps } from "@pureadmin/table";
import { reactive, ref, onMounted, h, toRaw, type Ref } from "vue";

export function useRole(tableRef: Ref) {
  const form = reactive({
    roleName: "",
    roleCode: "",
    isActive: null,
    phoneNumber: "",
    orderBy: "Id",
    sortDirection: "Descending",
    pageNumber: 1,
    pageSize: 10
  });
  const formRef = ref();
  const dataList = ref([]);
  const loading = ref(true);
  const switchLoadMap = ref({});
  const selectedNum = ref(0);
  const { switchStyle } = usePublicHooks();
  const pagination = reactive<PaginationProps>({
    total: 0,
    pageSize: 10,
    currentPage: 1,
    background: true
  });
  const treeData = ref([]);
  const columns: TableColumnList = [
    {
      label: "角色编号",
      prop: "id",
      minWidth: 120,
      formatter: row => {
        if (typeof row.id === "string" && row.id.length === 19) {
          // 如果 id 是字符串且长度为19，截取后10位并显示
          return row.id.substr(9); // 从第9位开始截取后10位
        } else {
          // 如果不符合要求，直接显示原始值
          return row.id;
        }
      }
    },
    {
      label: "角色名称",
      prop: "roleName",
      minWidth: 120
    },
    {
      label: "角色标识",
      prop: "roleCode",
      minWidth: 150
    },
    {
      label: "状态",
      minWidth: 130,
      cellRenderer: scope => (
        <el-switch
          size={scope.props.size === "small" ? "small" : "default"}
          loading={switchLoadMap.value[scope.index]?.loading}
          v-model={scope.row.isActive}
          active-value={true}
          inactive-value={false}
          active-text="已启用"
          inactive-text="已停用"
          inline-prompt
          style={switchStyle.value}
          onChange={() => onChange(scope as any)}
        />
      )
    },
    {
      label: "描述",
      prop: "description",
      minWidth: 150
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
      width: 240,
      slot: "operation"
    }
  ];

  function onChange({ row, index }) {
    ElMessageBox.confirm(
      `确认要<strong>${
        row.isActive === false ? "停用" : "启用"
      }</strong><strong style='color:var(--el-color-primary)'>${
        row.roleName
      }</strong>吗?`,
      "系统提示",
      {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning",
        dangerouslyUseHTMLString: true,
        draggable: true
      }
    )
      .then(() => {
        switchLoadMap.value[index] = Object.assign(
          {},
          switchLoadMap.value[index],
          {
            loading: true
          }
        );
        setTimeout(() => {
          switchLoadMap.value[index] = Object.assign(
            {},
            switchLoadMap.value[index],
            {
              loading: false
            }
          );
          message(
            `已${row.isActive === false ? "停用" : "启用"}${row.roleName}`,
            {
              type: "success"
            }
          );
        }, 300);
      })
      .catch(() => {
        row.isActive === true ? (row.isActive = false) : (row.isActive = true);
      });
  }

  async function handleDelete(row) {
    await onbatchDeleteRole({ roleIds: [row.roleId] });
    message(`删除成功`, { type: "success" });
    onSearch();
  }

  function handleSizeChange(val: number) {
    form.pageSize = val;
    onSearch();
  }

  function handleCurrentChange(val: number) {
    form.pageNumber = val;
    onSearch();
  }

  function handleSelectionChange(val) {
    selectedNum.value = val.length;
    // 重置表格高度
    tableRef.value.setAdaptive();
  }

  async function onSearch() {
    loading.value = true;
    const { data } = await getRoleList(toRaw(form));
    dataList.value = data.items;
    pagination.total = data.totalItems;
    pagination.pageSize = data.pageSize;
    pagination.currentPage = data.currentPage;

    setTimeout(() => {
      loading.value = false;
    }, 500);
  }

  const resetForm = formEl => {
    if (!formEl) return;
    formEl.resetFields();
    onSearch();
  };

  function openDialog(title = "新增", row?: FormItemProps) {
    addDialog({
      title: `${title}角色`,
      props: {
        formInline: {
          roleId: row?.roleId ?? "",
          roleName: row?.roleName ?? "",
          roleCode: row?.roleCode ?? "",
          description: row?.description ?? "",
          concurrencyStamp: row?.concurrencyStamp ?? ""
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
          done(); // 关闭弹框
          onSearch(); // 刷新表格数据
        }
        FormRef.validate(async valid => {
          if (valid) {
            // 表单规则校验通过
            if (title === "新增") {
              await addRole(curData);
              message(`添加成功`, { type: "success" });
            } else {
              await updateRole(curData);
              message(`修改成功`, { type: "success" });
            }
            onSearch();
            chores();
          }
        });
      }
    });
  }

  async function openMenuDialog(title = "菜单权限", row?: MenuFormItemProps) {
    addDialog({
      title: `为${row?.roleName}分配${title}`,
      props: {
        formInline: {
          roleId: row?.roleId ?? "",
          roleName: row?.roleName ?? "",
          permissionOptions: [],
          permissionIds: [],
          rolePermissionsData: [],
          concurrencyStamp: row?.concurrencyStamp ?? ""
        }
      },
      width: "40%",
      draggable: true,
      fullscreenIcon: true,
      closeOnClickModal: false,
      contentRenderer: () => h(menuForm, { ref: formRef }),
      beforeSure: (done, { options }) => {
        const curData = options.props.formInline as MenuFormItemProps;
        async function chores() {
          console.log(curData);
          await updateRolePermissionMenu(curData);
          message(`操作成功`, { type: "success" });
          onSearch();
          done(); // 关闭弹框
        }
        chores();
      }
    });
  }

  onMounted(() => {
    onSearch();
  });

  return {
    form,
    loading,
    columns,
    dataList,
    pagination,
    selectedNum,
    onSearch,
    resetForm,
    openDialog,
    openMenuDialog,
    handleDelete,
    handleSizeChange,
    handleCurrentChange,
    handleSelectionChange
  };
}
