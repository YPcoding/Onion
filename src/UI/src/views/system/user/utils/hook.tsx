import dayjs from "dayjs";
import {
  getUserList,
  onbatchDeleteUser,
  getAllRole,
  getAllUser,
  addUser,
  updateUser,
  updateUserAvatar,
  resetPassword,
  getAllRoleByUserId,
  roleAssigning
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
import { zxcvbn } from "@zxcvbn-ts/core";
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
  const ruleFormRef = ref();
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
          return row.phoneNumber;
        }
      }
    },
    {
      label: "角色",
      prop: "roles",
      minWidth: 90,
      formatter: row => {
        if (Array.isArray(row.roles)) {
          const soleStrings = row.roles.map(roleObj => {
            return `${roleObj.roleName}`;
          });
          return soleStrings.join(", ");
        } else {
          return row.roles;
        }
      }
    },
    {
      label: "账号状态",
      prop: "lockoutEnabled",
      minWidth: 90,
      cellRenderer: scope => (
        <el-switch
          size={scope.props.size === "small" ? "small" : "default"}
          loading={switchLoadMap.value[scope.index]?.loading}
          v-model={scope.row.lockoutEnabled}
          active-value={false}
          inactive-value={true}
          active-text="未锁定"
          inactive-text="已锁定"
          inline-prompt
          style={switchStyle.value}
        />
      )
    },
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

  async function handleDelete(row) {
    // 调用 onbatchDeleteUser 方法
    await onbatchDeleteUser({ userIds: [row.id] });
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

  /** 当CheckBox选择项发生变化时会触发该事件 */
  function handleSelectionChange(val) {
    selectedNum.value = val.length;
    // 重置表格高度
    tableRef.value.setAdaptive();
  }

  async function openDialog(title = "新增", row?: FormItemProps) {
    higherUserOptions.value = (await getAllUser()).data;
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
        FormRef.validate(async valid => {
          if (valid) {
            // 表单规则校验通过
            if (title === "新增") {
              await addUser(curData);
              message(`添加成功`, { type: "success" });
            } else {
              await updateUser(curData);
              message(`修改成功`, { type: "success" });
            }
            onSearch();
            chores();
          }
        });
      }
    });
  }

  /** 批量删除 */
  async function onbatchDel() {
    const curSelected = tableRef.value.getTableRef().getSelectionRows();
    // 调用 onbatchDeleteUser 方法
    await onbatchDeleteUser({ userIds: getKeyList(curSelected, "id") });
    message(`删除成功`, { type: "success" });
    onSearch();
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
      beforeSure: async done => {
        const base64Data = avatarInfo.value.base64; // 替换为实际的 Base64 数据
        const imageUrl = (await uploadEnclosure({ base64: base64Data })).data;
        await updateUserAvatar({
          userId: row?.userId,
          profilePictureDataUrl: imageUrl,
          concurrencyStamp: row?.concurrencyStamp
        });
        message(`修改成功`, { type: "success" });
        onSearch();
        done(); // 关闭弹框
      }
    });
  }

  // 重置的新密码
  const pwdForm = reactive({
    newPwd: ""
  });
  const pwdProgress = [
    { color: "#e74242", text: "非常弱" },
    { color: "#EFBD47", text: "弱" },
    { color: "#ffa500", text: "一般" },
    { color: "#1bbf1b", text: "强" },
    { color: "#008000", text: "非常强" }
  ];
  // 当前密码强度（0-4）
  const curScore = ref();

  watch(
    pwdForm,
    ({ newPwd }) =>
      (curScore.value = isAllEmpty(newPwd) ? -1 : zxcvbn(newPwd).score)
  );

  /** 重置密码 */
  async function handleReset(row) {
    addDialog({
      title: `重置 ${row.userName} 用户的密码`,
      width: "30%",
      draggable: true,
      closeOnClickModal: false,
      contentRenderer: () => (
        <>
          <ElForm ref={ruleFormRef} model={pwdForm}>
            <ElFormItem
              prop="newPwd"
              rules={[
                {
                  required: true,
                  message: "请输入新密码",
                  trigger: "blur"
                }
              ]}
            >
              <ElInput
                clearable
                show-password
                type="password"
                v-model={pwdForm.newPwd}
                placeholder="请输入新密码"
              />
            </ElFormItem>
          </ElForm>
          <div class="mt-4 flex">
            {pwdProgress.map(({ color, text }, idx) => (
              <div
                class="w-[19vw]"
                style={{ marginLeft: idx !== 0 ? "4px" : 0 }}
              >
                <ElProgress
                  striped
                  striped-flow
                  duration={curScore.value === idx ? 6 : 0}
                  percentage={curScore.value >= idx ? 100 : 0}
                  color={color}
                  stroke-width={10}
                  show-text={false}
                />
                <p
                  class="text-center"
                  style={{ color: curScore.value === idx ? color : "" }}
                >
                  {text}
                </p>
              </div>
            ))}
          </div>
        </>
      ),
      closeCallBack: () => (pwdForm.newPwd = ""),
      beforeSure: done => {
        ruleFormRef.value.validate(async valid => {
          if (valid) {
            await resetPassword({
              userId: row.userId,
              password: pwdForm.newPwd,
              concurrencyStamp: row.concurrencyStamp
            });
            // 表单规则校验通过
            message(`已成功重置 ${row.userName} 用户的密码`, {
              type: "success"
            });

            done(); // 关闭弹框
            onSearch(); // 刷新表格数据
          }
        });
      }
    });
  }

  /** 分配角色 */
  async function handleRole(row) {
    // 选中的角色列表
    const roleIds =
      (await getAllRoleByUserId(row.userId)).data?.map(item => item.roleId) ??
      [];
    addDialog({
      title: `分配 ${row.userName} 用户的角色`,
      props: {
        formInline: {
          userName: row?.userName ?? "",
          roleOptions: roleOptions.value ?? [],
          roleIds
        }
      },
      width: "400px",
      draggable: true,
      fullscreenIcon: true,
      closeOnClickModal: false,
      contentRenderer: () => h(roleForm),
      beforeSure: async (done, { options }) => {
        const curData = options.props.formInline as RoleFormItemProps;
        console.log("curIds", curData.roleIds);
        await roleAssigning({ userId: row.userId, roleIds: curData.roleIds });
        onSearch();
        message(`分配 ${row.userName} 用户角色成功`, {
          type: "success"
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
    handleReset,
    handleSizeChange,
    onSelectionCancel,
    handleCurrentChange,
    handleSelectionChange,
    handleRole
  };
}
