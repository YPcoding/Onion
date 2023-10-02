<script setup lang="ts">
import { ref } from "vue";
import ReCol from "@/components/ReCol";
import { formRules } from "../utils/rule";
import { FormProps } from "../utils/types";
import { usePublicHooks } from "@/views/system/hooks";
                        
const props = withDefaults(defineProps<FormProps>(), {
  formInline: () => ({
    /** 用于判断是`新增`还是`修改` */
    formTitle: "新增",
    notificationId: "",            
    title: "",
    content: "",
    link: "",                     
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
        <el-form-item label="通知标题" prop="title">
          <el-input
            v-model="newFormInline.title"
            clearable
            placeholder="请输入通知标题"
          />
        </el-form-item>
      </re-col>

      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="通知内容" prop="content">
          <el-input
            v-model="newFormInline.content"
            clearable
            placeholder="请输入通知内容"
          />
        </el-form-item>
      </re-col>

      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="相关链接">
          <el-input
            v-model="newFormInline.link"
            clearable
            placeholder="请输入相关链接"
          />
        </el-form-item>
      </re-col>                    
    </el-row>
  </el-form>
</template>
