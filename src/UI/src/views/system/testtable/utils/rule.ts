import { reactive } from "vue";
import type { FormRules } from "element-plus";

/** 自定义表单规则校验 */
export const formRules = reactive(<FormRules>{
  name: [{ required: true, message: "名称为必填项", trigger: "blur" }],
  description: [{ required: true, message: "描述为必填项", trigger: "blur" }],
  dateTime: [{ required: true, message: "时间为必填项", trigger: "blur" }]
});
