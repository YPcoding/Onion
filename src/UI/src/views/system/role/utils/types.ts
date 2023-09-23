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

export type { FormItemProps, FormProps };
