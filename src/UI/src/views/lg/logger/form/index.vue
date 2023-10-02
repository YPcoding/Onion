<script setup lang="ts">
import { ref } from "vue";
import ReCol from "@/components/ReCol";
import { formRules } from "../utils/rule";
import { FormProps } from "../utils/types";
import { usePublicHooks } from "@/views/system/hooks";
                        
const props = withDefaults(defineProps<FormProps>(), {
  formInline: () => ({
    /** 用于判断是`新增`还是`修改` */
    title: "新增",
    loggerId: "",

                        
    message: "",

    messageTemplate: "",

    level: "",
     
    timeStamp: "",

    exception: "",

    userName: "",

    clientIP: "",

    clientAgent: "",

    properties: "",

    logEvent: "",

                        
    concurrencyStamp: ""
  })
});

const ruleFormRef = ref();
const { switchStyle } = usePublicHooks();
const newFormInline = ref(props.formInline);

function getRef() {
  return ruleFormRef.value;
}

defineExpose({ getRef });
</script>

                        
<template>
  <el-form
    ref="ruleFormRef"
    :model="newFormInline"
    :rules="formRules"
    label-width="82px"
  >
    <el-row :gutter="30">

                        
      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="消息" prop="message">
          <el-input
            v-model="newFormInline.message"
            clearable
            placeholder="请输入消息"
          />
        </el-form-item>
      </re-col>

      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="消息模板" prop="messageTemplate">
          <el-input
            v-model="newFormInline.messageTemplate"
            clearable
            placeholder="请输入消息模板"
          />
        </el-form-item>
      </re-col>

      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="消息等级" prop="level">
          <el-input
            v-model="newFormInline.level"
            clearable
            placeholder="请输入消息等级"
          />
        </el-form-item>
      </re-col>

      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="发生时间" prop="timeStamp">
          <el-date-picker
            v-model="newFormInline.timeStamp"
            type="datetime"
            placeholder="选择一个发生时间"
          />
        </el-form-item>
      </re-col>

      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="异常" prop="exception">
          <el-input
            v-model="newFormInline.exception"
            clearable
            placeholder="请输入异常"
          />
        </el-form-item>
      </re-col>

      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="用户名" prop="userName">
          <el-input
            v-model="newFormInline.userName"
            clearable
            placeholder="请输入用户名"
          />
        </el-form-item>
      </re-col>

      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="客户端IP" prop="clientIP">
          <el-input
            v-model="newFormInline.clientIP"
            clearable
            placeholder="请输入客户端IP"
          />
        </el-form-item>
      </re-col>

      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="IP" prop="clientAgent">
          <el-input
            v-model="newFormInline.clientAgent"
            clearable
            placeholder="请输入IP"
          />
        </el-form-item>
      </re-col>

      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="特征" prop="properties">
          <el-input
            v-model="newFormInline.properties"
            clearable
            placeholder="请输入特征"
          />
        </el-form-item>
      </re-col>

      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="日志事件" prop="logEvent">
          <el-input
            v-model="newFormInline.logEvent"
            clearable
            placeholder="请输入日志事件"
          />
        </el-form-item>
      </re-col>

                        
    </el-row>
  </el-form>
</template>
