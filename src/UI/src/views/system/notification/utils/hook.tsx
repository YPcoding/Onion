//引入组件
import dayjs from "dayjs";
import {
  getNotificationList,
  addNotification,
  updateNotification,
  onbatchDeleteNotification
} from "@/api/system/notification";
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
export function useNotification(tableRef: Ref, treeRef: Ref) {
  //常量
  const form = reactive({
    title: "",

    content: "",

    link: "",

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
    const { data } = await getNotificationList(toRaw(form));
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
      label: "通知标题",
      prop: "title",
      minWidth: 100
    },

    {
      label: "通知内容",
      prop: "content",
      minWidth: 100
    },

    {
      label: "相关链接",
      prop: "link",
      minWidth: 100
    },

    {
      label: "操作",
      fixed: "right",
      width: 180,
      slot: "operation",
      hide: () => {
        // 判断权限是否可以显示操作栏
        const auths = ["api:notification:update", "api:notification:delete"];
        return !usePublicHooks().hasAuthIntersection(getAuths(), auths);
      }
    }
  ];
  //生命周期钩子函数
  onMounted(async () => {
    onSearch();
  });

  /** 新增或修改 */
  async function openDialog(formTitle = "新增", row?: FormItemProps) {
    addDialog({
      title: `${formTitle}通知`,
      props: {
        formInline: {
          formTitle,
          notificationId: row?.notificationId ?? "",

          title: row?.title ?? "",

          content: row?.content ?? "",

          link: row?.link ?? "",

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
          message(`${formTitle}成功`, { type: "success" });
          done(); // 关闭弹框
          onSearch(); // 刷新表格数据
        }
        FormRef.validate(async valid => {
          if (valid) {
            // 表单规则校验通过
            if (formTitle === "新增") {
              await addNotification(curData);
            } else {
              await updateNotification(curData);
            }
            chores();
          }
        });
      }
    });
  }

  //删除
  async function handleDelete(row) {
    await onbatchDeleteNotification({ notificationIds: [row.notificationId] });
    message(`删除成功`, { type: "success" });
    onSearch();
  }

  /** 批量删除 */
  async function onbatchDel() {
    const curSelected = tableRef.value.getTableRef().getSelectionRows();
    await onbatchDeleteNotification({
      notificationIds: getKeyList(curSelected, "id")
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
