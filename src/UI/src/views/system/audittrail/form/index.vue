<script setup lang="ts">
import { ref } from "vue";
import ReCol from "@/components/ReCol";
import { formRules } from "../utils/rule";
import { FormProps } from "../utils/types";
import { usePublicHooks } from "../../hooks";
                        
const props = withDefaults(defineProps<FormProps>(), {
  formInline: () => ({
    /** 用于判断是`新增`还是`修改` */
    title: "新增",
    auditTrailId: "",                      
    tableName: "",
    dateTime: "",    
    hasTemporaryProperties: null,       
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
        <el-form-item label="表名" prop="tableName">
          <el-input
            v-model="newFormInline.tableName"
            clearable
            placeholder="请输入表名"
          />
        </el-form-item>
      </re-col>

      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="审计时间" prop="dateTime">
          <el-date-picker
            v-model="newFormInline.dateTime"
            type="datetime"
            placeholder="选择一个审计时间"
          />
        </el-form-item>
      </re-col>

      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="具有临时属性">
          <el-switch
            v-model="newFormInline.hasTemporaryProperties"
            inline-prompt
            :active-value="true"
            :inactive-value="false"
            active-text="是"
            inactive-text="否"
            :style="switchStyle"
          />
        </el-form-item>
      </re-col>

                        
    </el-row>
  </el-form>
</template>
