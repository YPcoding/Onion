import dayjs from "dayjs";
import {
  getUserList,
  onbatchDeleteUser,
  UserDeleteParams
} from "@/api/system/user";
import { type PaginationProps } from "@pureadmin/table";
import { usePublicHooks } from "../../hooks";
import { message } from "@/utils/message";
import { hideTextAtIndex, getKeyList, isAllEmpty } from "@pureadmin/utils";
import {
  ElForm,
  ElInput,
  ElFormItem,
  ElProgress,
  ElMessageBox
} from "element-plus";
import {
  type Ref,
  h,
  ref,
  toRaw,
  watch,
  computed,
  reactive,
  onMounted
} from "vue";
import { bool } from "vue-types";

export function useUser(tableRef: Ref, treeRef: Ref) {
  const form = reactive({
    userName: "",
    email: "",
    emailConfirmed: null,
    phoneNumber: "",
    lockoutEnabled: null,
    orderBy: "Id",
    sortDirection: "Descending"
  });

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

  async function onSearch() {
    const { data } = await getUserList(toRaw(form));
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
    treeRef.value.onTreeReset();
    onSearch();
  };

  const columns: TableColumnList = [
    {
      label: "勾选列", // 如果需要表格多选，此处label必须设置
      type: "selection",
      fixed: "left",
      reserveSelection: true // 数据刷新后保留选项
    },
    {
      label: "用户编号",
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
      label: "用户头像",
      prop: "profilePictureDataUrl",
      cellRenderer: ({ row }) => (
        <el-image
          fit="cover"
          preview-teleported={true}
          src={row.profilePictureDataUrl}
          preview-src-list={Array.of(row.profilePictureDataUrl)}
          class="w-[24px] h-[24px] rounded-full align-middle"
        />
      ),
      width: 90
    },
    {
      label: "用户名称",
      prop: "userName",
      minWidth: 130
    },
    {
      label: "手机号码",
      prop: "phoneNumber",
      minWidth: 90,
      formatter: row => {
        if (row.phoneNumber) {
          return hideTextAtIndex(row.phoneNumber, { start: 3, end: 6 });
        } else {
          return row.phoneNumber; // 或者返回空字符串或其他默认值，具体根据需求而定
        }
      }
    },
    {
      label: "角色",
      prop: "roles", // 假设 emails 是包含多个对象的数组
      minWidth: 90,
      formatter: row => {
        if (Array.isArray(row.roles)) {
          // 如果 roles 是数组，则将其转换为字符串并显示
          const soleStrings = row.roles.map(roleObj => {
            // 根据对象的属性组合成字符串
            return `${roleObj.roleName}`;
          });
          return soleStrings.join(", ");
        } else {
          // 如果不是数组，则直接显示原始值
          return row.roles;
        }
      }
    },
    // {
    //   label: "邮箱地址",
    //   prop: "email",
    //   minWidth: 90,
    //   formatter: row => {
    //     if (row.email) {
    //       const atIndex = row.email.indexOf("@"); // 找到 @ 符号的位置
    //       if (atIndex >= 0) {
    //         const username = row.email.substring(0, atIndex); // @ 符号前的部分
    //         const domain = row.email.substring(atIndex); // @ 符号后的部分
    //         const hiddenUsername = `${username.charAt(0)}${"*".repeat(
    //           username.length - 1
    //         )}`; // 将用户名部分隐藏
    //         return `${hiddenUsername}${domain}`;
    //       } else {
    //         return row.email; // 如果没有 @ 符号，则返回原始值
    //       }
    //     } else {
    //       return row.email; // 或者返回空字符串或其他默认值，具体根据需求而定
    //     }
    //   }
    // },
    // {
    //   label: "号码状态",
    //   prop: "phoneNumberConfirmed",
    //   minWidth: 90,
    //   cellRenderer: scope => (
    //     <el-switch
    //       size={scope.props.size === "small" ? "small" : "default"}
    //       loading={switchLoadMap.value[scope.index]?.loading}
    //       v-model={scope.row.status}
    //       active-value={1}
    //       inactive-value={0}
    //       active-text="已确认"
    //       inactive-text="未确认"
    //       inline-prompt
    //       style={switchStyle.value}
    //     />
    //   )
    // },
    // {
    //   label: "邮箱状态",
    //   prop: "emailConfirmed",
    //   minWidth: 90,
    //   cellRenderer: scope => (
    //     <el-switch
    //       size={scope.props.size === "small" ? "small" : "default"}
    //       loading={switchLoadMap.value[scope.index]?.loading}
    //       v-model={scope.row.status}
    //       active-value={1}
    //       inactive-value={0}
    //       active-text="已确认"
    //       inactive-text="未确认"
    //       inline-prompt
    //       style={switchStyle.value}
    //     />
    //   )
    // },
    {
      label: "账号状态",
      prop: "lockoutEnabled",
      minWidth: 90,
      cellRenderer: scope => (
        <el-switch
          size={scope.props.size === "small" ? "small" : "default"}
          loading={switchLoadMap.value[scope.index]?.loading}
          v-model={scope.row.status}
          active-value={1}
          inactive-value={0}
          active-text="已锁定"
          inactive-text="未锁定"
          inline-prompt
          style={switchStyle.value}
        />
      )
    },
    // {
    //   label: "邮箱状态",
    //   prop: "emailConfirmed",
    //   minWidth: 90,
    //   cellRenderer: scope => (
    //     <el-switch
    //       size={scope.props.size === "small" ? "small" : "default"}
    //       loading={switchLoadMap.value[scope.index]?.loading}
    //       v-model={scope.row.status}
    //       active-value={1}
    //       inactive-value={0}
    //       active-text="已确认"
    //       inactive-text="未确认"
    //       inline-prompt
    //       style={switchStyle.value}
    //     />
    //   )
    // },
    {
      label: "创建时间",
      minWidth: 90,
      prop: "created",
      formatter: ({ created }) => dayjs(created).format("YYYY-MM-DD HH:mm:ss")
    },
    {
      label: "操作",
      fixed: "right",
      width: 180,
      slot: "operation"
    }
  ];
  const buttonClass = computed(() => {
    return [
      "!h-[20px]",
      "reset-margin",
      "!text-gray-500",
      "dark:!text-white",
      "dark:hover:!text-primary"
    ];
  });

  onMounted(async () => {
    onSearch();
  });

  function onChange({ row, index }) {}

  function handleUpdate(row) {
    console.log(row);
  }

  function handleDelete(row) {
    // 创建一个参数对象

    // 调用 onbatchDeleteUser 方法
    onbatchDeleteUser({ userIds: [row.id] })
      .then(response => {
        // 处理成功的响应
        if (response.succeeded && response.data) {
          message(`您删除了用户编号为${row.id}的这条数据`, {
            type: "success"
          });
          onSearch();
        } else {
          message(`删除失败`, {
            type: "error"
          });
        }
      })
      .catch(error => {
        // 处理错误
        message(error, {
          type: "error"
        });
      });
  }

  function handleSizeChange(val: number) {
    console.log(`${val} items per page`);
  }

  function handleCurrentChange(val: number) {
    console.log(`current page: ${val}`);
  }

  /** 当CheckBox选择项发生变化时会触发该事件 */
  function handleSelectionChange(val) {
    // 重置表格高度
    tableRef.value.setAdaptive();
  }

  /** 取消选择 */
  function onSelectionCancel() {
    // 用于多选表格，清空用户的选择
    tableRef.value.getTableRef().clearSelection();
  }

  /** 批量删除 */
  function onbatchDel() {
    const curSelected = tableRef.value.getTableRef().getSelectionRows();

    tableRef.value.getTableRef().clearSelection();
  }

  function onTreeSelect({ id, selected }) {
    onSearch();
  }

  function formatHigherDeptOptions(treeList) {
    // 根据返回数据的status字段值判断追加是否禁用disabled字段，返回处理后的树结构，用于上级部门级联选择器的展示（实际开发中也是如此，不可能前端需要的每个字段后端都会返回，这时需要前端自行根据后端返回的某些字段做逻辑处理）
    if (!treeList || !treeList.length) return;
    const newTreeList = [];
    for (let i = 0; i < treeList.length; i++) {
      treeList[i].disabled = treeList[i].status === 0 ? true : false;
      formatHigherDeptOptions(treeList[i].children);
      newTreeList.push(treeList[i]);
    }
    return newTreeList;
  }

  function openDialog(title = "新增", row?: FormItemProps) {}

  /** 上传头像 */
  function handleUpload(row) {}

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
    onTreeSelect,
    handleUpdate,
    handleDelete,
    handleUpload,
    handleSizeChange,
    onSelectionCancel,
    handleCurrentChange,
    handleSelectionChange
  };
}
