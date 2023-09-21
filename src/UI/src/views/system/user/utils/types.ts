interface FormItemProps {
  userId?: string;
  /** 用于判断是`新增`还是`修改` */
  title: string;
  superiorId: number;
  userName: string;
  password: string;
  confirmPassword: string;
  phoneNumber: string | number;
  email: string;
  isActive: boolean;
  roleOptions: any[];
  /** 选中的角色列表 */
  roleIds: Record<string, unknown>[];
  higherUserOptions: Record<string, unknown>[];
  concurrencyStamp: string;
}
interface FormProps {
  formInline: FormItemProps;
}

interface RoleFormItemProps {
  username: string;
  /** 角色列表 */
  roleOptions: any[];
  /** 选中的角色列表 */
  roleids: Record<number, unknown>[];
}
interface RoleFormProps {
  formInline: RoleFormItemProps;
}

export type { FormItemProps, FormProps, RoleFormItemProps, RoleFormProps };
