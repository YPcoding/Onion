interface FormItemProps {
  loggerId?: string;
  /** 用于判断是`新增`还是`修改` */
  title: string;
  message: string;

  messageTemplate: string;

  level: string;

  timeStamp: string;

  exception: string;

  userName: string;

  clientIP: string;

  clientAgent: string;

  properties: string;

  logEvent: string;
  concurrencyStamp: string;
}interface FormProps {formInline: FormItemProps;
}

export type {FormItemProps,FormProps};