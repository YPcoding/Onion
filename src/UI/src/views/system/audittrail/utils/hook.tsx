//引入组件
import dayjs from "dayjs";
import {
  getAuditTrailList,
  addAuditTrail,
  updateAuditTrail,
  onbatchDeleteAuditTrail
} from "@/api/system/audittrail";
import { type PaginationProps } from "@pureadmin/table";
import { usePublicHooks } from "../../hooks";
import { message } from "@/utils/message";
import { getKeyList } from "@pureadmin/utils";
import type { FormItemProps } from "../utils/types";
import { addDialog } from "@/components/ReDialog";
import editForm from "../form/index.vue";
import { type Ref, h, ref, toRaw, computed, reactive, onMounted } from "vue";
import { getAuths } from "@/router/utils";
//功能
export function useAuditTrail(tableRef: Ref, treeRef: Ref) {
  //常量
  const form = reactive({
    tableName: "",

    hasTemporaryProperties: null,

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

  //分页查询
  async function onSearch() {
    const { data } = await getAuditTrailList(toRaw(form));
    dataList.value =data.items;
    console.log(dataList.value);
    pagination.total = data.totalItems;
    pagination.pageSize = data.pageSize;
    pagination.currentPage = data.currentPage;

    setTimeout(() => {
      loading.value = false;
    }, 500);
  }

  //重置查询
  const resetForm = formEl => {
    if (!formEl) return;
    formEl.resetFields();
    treeRef.value.onTreeReset();
    onSearch();
  };

  //改变页码大小
  function handleSizeChange(val: number) {
    form.pageSize = val;
    onSearch();
  }

  //跳到指定页码
  function handleCurrentChange(val: number) {
    form.pageNumber = val;
    onSearch();
  }
  //数据列表行
  const columns: TableColumnList = [
    {
      label: "勾选列", // 如果需要表格多选，此处label必须设置
      type: "selection",
      fixed: "left",
      reserveSelection: true // 数据刷新后保留选项
    },
    {
      label: "编号",
      prop: "id", // 假设 id 包含19位的雪花ID
      width: 102,
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
      label: "表名",
      prop: "tableName",
      minWidth: 100
    },

    {
      label: "审计时间",
      minWidth: 100,
      prop: "dateTime",
      formatter: ({ dateTime }) => dayjs(dateTime).format("YYYY-MM-DD HH:mm:ss")
    },

    {
      label: "具有临时属性",
      prop: "hasTemporaryProperties",
      minWidth: 100,
      cellRenderer: scope => (
        <el-switch
          size={scope.props.size === "small" ? "small" : "default"}
          loading={switchLoadMap.value[scope.index]?.loading}
          v-model={scope.row.hasTemporaryProperties}
          active-value={true}
          inactive-value={false}
          active-text="是"
          inactive-text="否"
          inline-prompt
          style={switchStyle.value}
          onChange={() => handleHasTemporaryPropertiesOnChange(scope.row)}
        />
      )
    },

    {
      label: "操作",
      fixed: "right",
      width: 180,
      slot: "operation",
      hide: () => {
        // 判断权限是否可以显示操作栏
        const auths = ["api:audittrail:update", "api:audittrail:delete"];
        return !usePublicHooks().hasAuthIntersection(getAuths(), auths);
      }
    }
  ];
  //生命周期钩子函数
  onMounted(async () => {
    onSearch();
  });

  /** 新增或修改 */
  async function openDialog(title = "新增", row?: FormItemProps) {
    addDialog({
      title: `${title}审计日志`,
      props: {
        formInline: {
          title,
          auditTrailId: row?.auditTrailId ?? "",

          tableName: row?.tableName ?? "",

          dateTime: row?.dateTime ?? null,

          hasTemporaryProperties: row?.hasTemporaryProperties ?? true,

          concurrencyStamp: row?.concurrencyStamp ?? ""
        }
      },
      width: "46%",
      draggable: true,
      fullscreenIcon: true,
      closeOnClickModal: false,
      contentRenderer: () => h(editForm, { ref: formRef }),
      beforeSure: (done, { options }) => {
        const FormRef = formRef.value.getRef();
        const curData = options.props.formInline as FormItemProps;
        async function chores() {
          message(`${title}成功`, { type: "success" });
          done(); // 关闭弹框
          onSearch(); // 刷新表格数据
        }
        FormRef.validate(async valid => {
          if (valid) {
            // 表单规则校验通过
            if (title === "新增") {
              await addAuditTrail(curData);
            } else {
              await updateAuditTrail(curData);
            }
            chores();
          }
        });
      }
    });
  }

  //删除
  async function handleDelete(row) {
    await onbatchDeleteAuditTrail({ auditTrailIds: [row.auditTrailId] });
    message(`删除成功`, { type: "success" });
    onSearch();
  }

  /** 批量删除 */
  async function onbatchDel() {
    const curSelected = tableRef.value.getTableRef().getSelectionRows();
    await onbatchDeleteAuditTrail({
      auditTrailIds: getKeyList(curSelected, "id")
    });
    message(`删除成功`, { type: "success" });
    onSearch();
    tableRef.value.getTableRef().clearSelection();
  }

  /** 选择行 */
  function handleSelectionChange(val) {
    selectedNum.value = val.length;
    // 重置表格高度
    tableRef.value.setAdaptive();
  }

  /** 取消选择 */
  function onSelectionCancel() {
    selectedNum.value = 0;
    // 用于多选表格，清空用户的选择
    tableRef.value.getTableRef().clearSelection();
  }

  //数据列表自定义生成函数
  async function handleHasTemporaryPropertiesOnChange(row) {
    message(`功能未实现`, { type: "success" });
  }

  //按钮样式类
  const buttonClass = computed(() => {
    return [
      "!h-[20px]",
      "reset-margin",
      "!text-gray-500",
      "dark:!text-white",
      "dark:hover:!text-primary"
    ];
  });

  //返回数据
  return {
    form,
    loading,
    columns,
    dataList,
    pagination,
    selectedNum,
    buttonClass,
    onSearch,
    resetForm,
    onbatchDel,
    openDialog,
    handleDelete,
    handleSizeChange,
    onSelectionCancel,
    handleCurrentChange,
    handleSelectionChange
  };
}
