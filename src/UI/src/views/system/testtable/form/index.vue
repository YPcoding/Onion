<script setup lang="ts">
import { ref } from "vue";
import ReCol from "../../../../components/ReCol";
import { formRules } from "../utils/rule";
import { FormProps } from "../utils/types";
import { usePublicHooks } from "../../hooks";

const props = withDefaults(defineProps<FormProps>(), {
  formInline: () => ({
    /** 用于判断是`新增`还是`修改` */
    title: "新增",
    testTableId: "",
    name: "",
    dateTime: "",
    type: 0,
    stuts: true,
    description: "",
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
        <el-form-item label="名称" prop="name">
          <el-input
            v-model="newFormInline.name"
            clearable
            placeholder="请输入名称"
          />
        </el-form-item>
      </re-col>
      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="描述" prop="description">
          <el-input
            v-model="newFormInline.description"
            clearable
            placeholder="请输入描述"
          />
        </el-form-item>
      </re-col>
      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="时间" prop="dateTime">
          <el-input
            v-model="newFormInline.dateTime"
            clearable
            placeholder="请输入时间"
            type="date"
          />
        </el-form-item>
      </re-col>

      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="类型" prop="type">
          <el-input
            v-model="newFormInline.type"
            clearable
            placeholder="请输入类型"
            type="number"
          />
        </el-form-item>
      </re-col>
      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="状态">
          <el-switch
            v-model="newFormInline.stuts"
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
