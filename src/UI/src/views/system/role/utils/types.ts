interface FormItemProps {
  roleId: number;
  roleName: string;
  roleCode: string;
  description: string;
  isActive: boolean;
  concurrencyStamp: string;
}
interface FormProps {
  formInline: FormItemProps;
}

interface MenuFormItemProps {
  roleId: number;
  roleName: string;
  permissionOptions: any[];
  rolePermissionsData: Record<string, unknown>[];
}
interface MenuFormProps {
  formInline: MenuFormItemProps;
}

export type { FormItemProps, FormProps, MenuFormItemProps, MenuFormProps };
