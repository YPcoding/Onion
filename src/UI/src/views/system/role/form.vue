<script setup lang="ts">
import { ref } from "vue";
import { formRules } from "./utils/rule";
import { FormProps } from "./utils/types";

const props = withDefaults(defineProps<FormProps>(), {
  formInline: () => ({
    roleId: 0,
    roleName: "",
    roleCode: "",
    description: "",
    isActive: true,
    concurrencyStamp: ""
  })
});

const ruleFormRef = ref();
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
    <el-form-item label="角色名称" prop="roleName">
      <el-input
        v-model="newFormInline.roleName"
        clearable
        placeholder="请输入角色名称"
      />
    </el-form-item>

    <el-form-item label="角色标识" prop="roleCode">
      <el-input
        v-model="newFormInline.roleCode"
        clearable
        placeholder="请输入角色标识"
      />
    </el-form-item>

    <el-form-item label="描述" prop="description">
      <el-input
        v-model="newFormInline.description"
        placeholder="请输入描述信息"
        type="textarea"
      />
    </el-form-item>
  </el-form>
</template>
