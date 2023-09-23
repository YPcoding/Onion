interface FormItemProps {
  higherPermissionOptions: Record<number, unknown>[];
  id: number;
  permissionId: number;
  superiorId: number;
  label: string;
  type: number;
  path: string;
  httpMethods: string;
  icon: string;
  hidden: boolean;
  enabled: boolean;
  closable: boolean;
  opened: boolean;
  newWindow: boolean;
  external: boolean;
  sort: number;
  description: string;
  group: string;
  concurrencyStamp: string;
}
interface FormProps {
  formInline: FormItemProps;
}

export type { FormItemProps, FormProps };
