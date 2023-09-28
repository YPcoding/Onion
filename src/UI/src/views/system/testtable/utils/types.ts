interface FormItemProps {
  testTableId?: string;
  /** 用于判断是`新增`还是`修改` */
  title: string;
  name: string;

  description: string;

  dateTime: string;

  type: number;

  stuts: boolean;
  concurrencyStamp: string;
}interface FormProps {formInline: FormItemProps;
}

export type {FormItemProps,FormProps};