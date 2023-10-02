import { reactive } from "vue";
import type { FormRules } from "element-plus";

/** 自定义表单规则校验 */
export const formRules = reactive(<FormRules>{
  message: [{ required: true, message: "消息为必填项", trigger: "blur" }],

  messageTemplate: [{ required: true, message: "消息模板为必填项", trigger: "blur" }],

  level: [{ required: true, message: "消息等级为必填项", trigger: "blur" }],

  exception: [{ required: true, message: "异常为必填项", trigger: "blur" }],

  userName: [{ required: true, message: "用户名为必填项", trigger: "blur" }],

  clientIP: [{ required: true, message: "客户端IP为必填项", trigger: "blur" }],

  clientAgent: [{ required: true, message: "IP为必填项", trigger: "blur" }],

  properties: [{ required: true, message: "特征为必填项", trigger: "blur" }],

  logEvent: [{ required: true, message: "日志事件为必填项", trigger: "blur" }],
});