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
  profilePictureDataUrl: string;
}
interface FormProps {
  formInline: FormItemProps;
}

interface RoleFormItemProps {
  userId: string;
  userName: string;
  roleOptions: any[];
  roleIds: Record<number, unknown>[];
}
interface RoleFormProps {
  formInline: RoleFormItemProps;
}

export type { FormItemProps, FormProps, RoleFormItemProps, RoleFormProps };
