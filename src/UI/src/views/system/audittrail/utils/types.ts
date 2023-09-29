interface FormItemProps {
  auditTrailId?: string;
  /** 用于判断是`新增`还是`修改` */
  title: string;
  tableName: string;

  dateTime: string;

  hasTemporaryProperties: boolean;
  concurrencyStamp: string;
}interface FormProps {formInline: FormItemProps;
}

export type {FormItemProps,FormProps};