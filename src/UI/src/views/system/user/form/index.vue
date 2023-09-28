<script setup lang="ts">
import { ref } from "vue";
import ReCol from "../../../../components/ReCol";
import { formRules } from "../utils/rule";
import { FormProps } from "../utils/types";
import { usePublicHooks } from "../../hooks";

const props = withDefaults(defineProps<FormProps>(), {
  formInline: () => ({
    title: "新增",
    superiorId: null,
    userName: "",
    password: "",
    confirmPassword: "",
    phoneNumber: "",
    email: "",
    isActive: true,
    roleOptions: [],
    roleIds: [],
    higherUserOptions: [],
    concurrencyStamp: "",
    profilePictureDataUrl: ""
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
        <el-form-item label="用户名称" prop="userName">
          <el-input
            v-model="newFormInline.userName"
            clearable
            placeholder="请输入用户名称"
          />
        </el-form-item>
      </re-col>

      <re-col
        :value="12"
        :xs="24"
        :sm="24"
        v-if="newFormInline.title === '新增'"
      >
        <el-form-item label="用户密码" prop="password">
          <el-input
            v-model="newFormInline.password"
            clearable
            placeholder="请输入用户密码"
          />
        </el-form-item>
      </re-col>
      <re-col
        :value="12"
        :xs="24"
        :sm="24"
        v-if="newFormInline.title === '新增'"
      >
        <el-form-item label="确认密码" prop="confirmPassword">
          <el-input
            v-model="newFormInline.confirmPassword"
            clearable
            placeholder="请输入再次输入密码"
          />
        </el-form-item>
      </re-col>
      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="手机号码" prop="phoneNumber">
          <el-input
            v-model="newFormInline.phoneNumber"
            clearable
            placeholder="请输入手机号码"
          />
        </el-form-item>
      </re-col>

      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="邮箱地址" prop="email">
          <el-input
            v-model="newFormInline.email"
            clearable
            placeholder="请输入邮箱地址"
          />
        </el-form-item>
      </re-col>
      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="激活">
          <el-switch
            v-model="newFormInline.isActive"
            inline-prompt
            :active-value="true"
            :inactive-value="false"
            active-text="已激活"
            inactive-text="未激活"
            :style="switchStyle"
          />
        </el-form-item>
      </re-col>
      <re-col>
        <el-form-item label="角色列表" prop="roleIds">
          <el-select
            v-model="newFormInline.roleIds"
            placeholder="请选择"
            class="w-full"
            clearable
            multiple
          >
            <el-option
              v-for="(item, index) in newFormInline.roleOptions"
              :key="index"
              :value="item.roleId"
              :label="item.roleName"
            >
              {{ item.roleName }}
            </el-option>
          </el-select>
        </el-form-item>
      </re-col>
      <re-col>
        <el-form-item label="上级用户">
          <el-cascader
            class="w-full"
            v-model="newFormInline.superiorId"
            :options="newFormInline.higherUserOptions"
            :props="{
              value: 'userId',
              label: 'userName',
              emitPath: false,
              checkStrictly: true
            }"
            clearable
            filterable
            placeholder="请选择上级用户"
          >
            <template #default="{ node, data }">
              <span>{{ data.userName }}</span>
              <span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
            </template>
          </el-cascader>
        </el-form-item>
      </re-col>
    </el-row>
  </el-form>
</template>
