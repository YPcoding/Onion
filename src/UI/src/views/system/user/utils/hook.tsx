import dayjs from "dayjs";
import {
  getUserList,
  onbatchDeleteUser,
  getAllRole,
  getAllUser,
  addUser,
  updateUser,
  updateUserAvatar
} from "@/api/system/user";
import { uploadEnclosure, convertImageToBase64 } from "@/api/upload";
import { type PaginationProps } from "@pureadmin/table";
import { usePublicHooks } from "../../hooks";
import { message } from "@/utils/message";
import { hideTextAtIndex, getKeyList, isAllEmpty } from "@pureadmin/utils";
import type { FormItemProps, RoleFormItemProps } from "../utils/types";
import croppingUpload from "../upload.vue";
import { addDialog } from "@/components/ReDialog";
import editForm from "../form/index.vue";
import roleForm from "../form/role.vue";
import { cloneDeep } from "@pureadmin/utils";
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
import { error } from "console";

export function useUser(tableRef: Ref, treeRef: Ref) {
  const form = reactive({
    userName: "",
    email: "",
    emailConfirmed: null,
    phoneNumber: "",
    lockoutEnabled: null,
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
  const roleOptions = ref([]);
  const higherUserOptions = ref();
  const avatarInfo = ref();
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
          src={`${row.profilePictureDataUrl}`}
          preview-src-list={Array.of(`${row.profilePictureDataUrl}`)}
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
          v-model={scope.row.lockoutEnabled}
          active-value={true}
          inactive-value={false}
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
    // 角色列表
    roleOptions.value = (await getAllRole()).data;
    // userOptions.value = (await getAllUser()).data;
  });

  function handleUpdate(row) {}

  function handleDelete(row) {
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
    form.pageSize = val;
    onSearch();
  }

  function handleCurrentChange(val: number) {
    form.pageNumber = val;
    onSearch();
  }

  async function openDialog(title = "新增", row?: FormItemProps) {
    higherUserOptions.value = (await getAllUser()).data;
    console.log(higherUserOptions.value);
    addDialog({
      title: `${title}用户`,
      props: {
        formInline: {
          title,
          higherUserOptions: formatUserOptions(
            cloneDeep(higherUserOptions.value)
          ),
          userId: row?.userId ?? null,
          superiorId: row?.superiorId ?? null,
          userName: row?.userName ?? "",
          password: row?.password ?? "",
          confirmPassword: row?.confirmPassword ?? "",
          phoneNumber: row?.phoneNumber ?? "",
          email: row?.email ?? "",
          isActive: row?.isActive ?? null,
          roleIds: row?.roleIds ?? null,
          roleOptions: roleOptions.value ?? [],
          concurrencyStamp: row?.concurrencyStamp ?? "",
          profilePictureDataUrl: row?.profilePictureDataUrl
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
          done(); // 关闭弹框
          onSearch(); // 刷新表格数据
        }
        FormRef.validate(valid => {
          if (valid) {
            // 表单规则校验通过
            if (title === "新增") {
              // 实际开发先调用新增接口，再进行下面操作
              addUser(curData)
                .then(response => {
                  // 处理成功的响应
                  if (response.succeeded) {
                    onSearch();
                  } else {
                    message(`新增失败`, {
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
              chores();
            } else {
              // 实际开发先调用编辑接口，再进行下面操作
              updateUser(curData)
                .then(response => {
                  // 处理成功的响应
                  if (response.succeeded) {
                    onSearch();
                  } else {
                    message(`修改失败`, {
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
              chores();
            }
          }
        });
      }
    });
  }

  /** 批量删除 */
  function onbatchDel() {
    const curSelected = tableRef.value.getTableRef().getSelectionRows();
    // 调用 onbatchDeleteUser 方法
    onbatchDeleteUser({ userIds: getKeyList(curSelected, "id") })
      .then(response => {
        // 处理成功的响应
        if (response.succeeded && response.data) {
          message(`已删除用户编号为 ${getKeyList(curSelected, "id")} 的数据`, {
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
    tableRef.value.getTableRef().clearSelection();
  }

  function onTreeSelect({ id, selected }) {
    onSearch();
  }

  function formatUserOptions(data, superiorId?: string) {
    if (!data || !data.length) return;
    const tree = [];
    for (const item of data) {
      if (item.superiorId === superiorId) {
        const children = formatUserOptions(data, item.id);
        if (children.length > 0) {
          item.children = children;
        }
        tree.push(item);
      }
    }
    return tree;
  }
  /** 当CheckBox选择项发生变化时会触发该事件 */
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

  /** 上传头像 */
  async function handleUpload(row) {
    const imageUrl = `${row.profilePictureDataUrl}`;
    const base64Data2 = (await convertImageToBase64({ imagePath: imageUrl }))
      .data;
    addDialog({
      title: "裁剪、上传头像",
      width: "40%",
      draggable: true,
      closeOnClickModal: false,
      contentRenderer: () =>
        h(croppingUpload, {
          imgSrc: base64Data2,
          onCropper: info => (avatarInfo.value = info)
        }),
      beforeSure: done => {
        const base64Data = avatarInfo.value.base64; // 替换为实际的 Base64 数据
        uploadEnclosure({ base64: base64Data })
          .then(response => {
            // 处理成功的响应
            if (response.succeeded) {
              updateUserAvatar({
                userId: row?.userId,
                profilePictureDataUrl: response.data,
                concurrencyStamp: row?.concurrencyStamp
              })
                .then(res => {
                  if (res.succeeded) {
                    message(`修改成功`, {
                      type: "success"
                    });
                    onSearch(); // 刷新表格数据
                  } else {
                    message(`修改失败`, {
                      type: "error"
                    });
                  }
                })
                .catch(error => {
                  message(`修改失败`, {
                    type: "error"
                  });
                });
            } else {
              message(`上传失败`, {
                type: "error"
              });
            }
          })
          .catch(error => {
            // 处理错误
            message("上传失败", {
              type: "error"
            });
          });
        done(); // 关闭弹框
      }
    });
  }

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
