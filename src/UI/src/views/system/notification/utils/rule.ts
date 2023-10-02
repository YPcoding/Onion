import { reactive } from "vue";
import type { FormRules } from "element-plus";

/** 自定义表单规则校验 */
export const formRules = reactive(<FormRules>{
  title: [{ required: true, message: "通知标题为必填项", trigger: "blur" }],

  content: [{ required: true, message: "通知内容为必填项", trigger: "blur" }],

  link: [{ required: true, message: "相关链接为必填项", trigger: "blur" }],
});