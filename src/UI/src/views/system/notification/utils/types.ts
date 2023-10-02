interface FormItemProps {
  notificationId?: string;
  /** 用于判断是`新增`还是`修改` */
  formTitle: string;
  title: string;

  content: string;

  link: string;
  concurrencyStamp: string;
}
interface FormProps {
  formInline: FormItemProps;
}

export type { FormItemProps, FormProps };
