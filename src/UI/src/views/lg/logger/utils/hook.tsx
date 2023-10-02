//引入组件
import dayjs from "dayjs";
import {
  getLoggerList,
  addLogger,
  updateLogger,
  onbatchDeleteLogger
} from "@/api/lg/logger";
import { type PaginationProps } from "@pureadmin/table";
import { usePublicHooks } from "@/views/system/hooks";
import { message } from "@/utils/message";
import { getKeyList } from "@pureadmin/utils";
import type { FormItemProps } from "../utils/types";
import { addDialog } from "@/components/ReDialog";
import editForm from "../form/index.vue";
import { type Ref, h, ref, toRaw, computed, reactive, onMounted } from "vue";
import { getAuths } from "@/router/utils";
//功能
export function useLogger(tableRef: Ref, treeRef: Ref) {
  //常量
  const form = reactive({
    message: "",

    messageTemplate: "",

    level: "",

    exception: "",

    userName: "",

    clientIP: "",

    clientAgent: "",

    properties: "",

    logEvent: "",

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

  function formatDataItems(data) {
    if (!data || !data.length) return;
    const items = [];
    for (const item of data) {
      if (item.logEvent !== null) {
        const response = item.logEvent;
        const decodedString = response.replace(
          /\\u([\d\w]{4})/gi,
          (match, grp) => String.fromCharCode(parseInt(grp, 16))
        );
        console.log(decodedString);
        const replacedString = decodedString.replace(/\\/g, "");
        items.push({
          id: item.id,
          loggerId: item.loggerId,
          timeStamp: item.timeStamp,
          level: item.level,
          logEvent: replacedString,
          userName: item.userName,
          clientIP: item.clientIP,
          clientAgent: item.clientAgent
        });
      }
    }
    return items;
  }

  //分页查询
  async function onSearch() {
    const { data } = await getLoggerList(toRaw(form));
    dataList.value = formatDataItems(data.items);
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
      label: "用户名",
      prop: "userName",
      minWidth: 100
    },
    {
      label: "消息等级",
      prop: "level",
      minWidth: 100,
      cellRenderer: ({ row }) => {
        let tagType = "info"; // 默认标签类型为 "info"
        // 根据 row.level 的不同值来设置不同的标签类型
        if (row.level === "Information") {
          tagType = "";
        } else if (row.level === "Warning") {
          tagType = "warning";
        } else if (row.level === "Error") {
          tagType = "error";
        } // 可以根据需要添加更多条件

        return (
          <el-tag class="ml-2" type={tagType}>
            {row.level}
          </el-tag>
        );
      }
    },
    {
      label: "发生时间",
      minWidth: 100,
      prop: "timeStamp",
      formatter: ({ timeStamp }) =>
        dayjs(timeStamp).format("YYYY-MM-DD HH:mm:ss")
    },

    {
      label: "客户端IP",
      prop: "clientIP",
      minWidth: 100
    },

    {
      label: "IP",
      prop: "clientAgent",
      minWidth: 100
    }
  ];
  //生命周期钩子函数
  onMounted(async () => {
    onSearch();
  });

  /** 新增或修改 */
  async function openDialog(title = "新增", row?: FormItemProps) {
    addDialog({
      title: `${title}日志`,
      props: {
        formInline: {
          title,
          loggerId: row?.loggerId ?? "",

          message: row?.message ?? "",

          messageTemplate: row?.messageTemplate ?? "",

          level: row?.level ?? "",

          timeStamp: row?.timeStamp ?? null,

          exception: row?.exception ?? "",

          userName: row?.userName ?? "",

          clientIP: row?.clientIP ?? "",

          clientAgent: row?.clientAgent ?? "",

          properties: row?.properties ?? "",

          logEvent: row?.logEvent ?? "",

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
              await addLogger(curData);
            } else {
              await updateLogger(curData);
            }
            chores();
          }
        });
      }
    });
  }

  //删除
  async function handleDelete(row) {
    await onbatchDeleteLogger({ loggerIds: [row.loggerId] });
    message(`删除成功`, { type: "success" });
    onSearch();
  }

  /** 批量删除 */
  async function onbatchDel() {
    const curSelected = tableRef.value.getTableRef().getSelectionRows();
    await onbatchDeleteLogger({ loggerIds: getKeyList(curSelected, "id") });
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
