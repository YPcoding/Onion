interface FormItemProps {
  testTableId?: string;
  id?: string;
  /** 用于判断是`新增`还是`修改` */
  title: string;
  name: string;
  dateTime: string;
  type: number;
  stuts: boolean;
  description: string;
  concurrencyStamp: string;
}
interface FormProps {
  formInline: FormItemProps;
}

export type { FormItemProps, FormProps };
