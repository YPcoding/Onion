//引入组件
import dayjs from "dayjs";
import {
  getAuditTrailList,
  addAuditTrail,
  updateAuditTrail,
  onbatchDeleteAuditTrail
} from "@/api/lg/audittrail";
import { type PaginationProps } from "@pureadmin/table";
import { usePublicHooks } from "@/views/system/hooks";
import { message } from "@/utils/message";
import { getKeyList } from "@pureadmin/utils";
import { type Ref, h, ref, toRaw, computed, reactive, onMounted } from "vue";
import { getAuths } from "@/router/utils";
//功能
export function useAuditTrail(tableRef: Ref, treeRef: Ref) {
  //常量
  const form = reactive({
    tableName: "",

    hasTemporaryProperties: null,

    orderBy: "Id",
    sortDirection: "Ascending",
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
    dataList.value = data.items;
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
      type: "expand",
      slot: "expand"
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
      label: "操作用户",
      prop: "owner.userName",
      minWidth: 100
    },
    {
      label: "表名",
      prop: "tableName",
      minWidth: 100
    },
    {
      label: "类型",
      prop: "auditType",
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
    }
  ];
  //生命周期钩子函数
  onMounted(async () => {
    onSearch();
  });

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
    handleDelete,
    handleSizeChange,
    onSelectionCancel,
    handleCurrentChange,
    handleSelectionChange
  };
}
