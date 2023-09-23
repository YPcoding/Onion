<script setup lang="ts">
import { ref, onBeforeMount } from "vue";
import ReCol from "@/components/ReCol";
import { formRules } from "./utils/rule";
import { FormProps } from "./utils/types";
import { usePublicHooks } from "../hooks";
import { message } from "@/utils/message";
import Segmented, { type OptionsType } from "@/components/ReSegmented";

const props = withDefaults(defineProps<FormProps>(), {
  formInline: () => ({
    higherPermissionOptions: [],
    id: 0,
    permissionId: 0,
    superiorId: 0,
    label: "",
    type: 0,
    path: "",
    httpMethods: "",
    icon: "",
    hidden: null,
    enabled: null,
    closable: null,
    opened: null,
    newWindow: null,
    external: null,
    sort: 0,
    description: "",
    group: ""
  })
});

const ruleFormRef = ref();
const { switchStyle } = usePublicHooks();
const newFormInline = ref(props.formInline);
let optionsPermissionTypeValue: number;
let optionsHttpMethodsValue: number;

const optionsHttpMethods: Array<OptionsType> = [
  {
    label: "无",
    value: null
  },
  {
    label: "GET",
    value: "GET"
  },
  {
    label: "POST",
    value: "POST"
  },
  {
    label: "PUT",
    value: "PUT"
  },
  {
    label: "DELETE",
    value: "DELETE"
  }
];

const optionsPermissionType: Array<OptionsType> = [
  {
    label: "菜单",
    value: 10
  },
  {
    label: "页面",
    value: 20
  },
  {
    label: "权限点",
    value: 30
  }
];

function getRef() {
  return ruleFormRef.value;
}

function onPermissionTypeChange({ index, option }) {
  const { label, value } = option;
  newFormInline.value.type = value;
}

function onHttpMethodsChange({ index, option }) {
  const { label, value } = option;
  newFormInline.value.httpMethods = label;
}

onBeforeMount(async () => {
  optionsPermissionTypeValue = optionsPermissionType.findIndex(
    item => item.value === newFormInline.value.type
  );
  if (optionsPermissionTypeValue === -1) {
    optionsPermissionTypeValue = 0;
  }
  optionsHttpMethodsValue = optionsHttpMethods.findIndex(
    item => item.value === newFormInline.value.httpMethods
  );
  if (optionsHttpMethodsValue === -1) {
    optionsHttpMethodsValue = 0;
  }
});

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
      <re-col>
        <el-form-item label="权限类型" prop="type">
          <Segmented
            :options="optionsPermissionType"
            @change="onPermissionTypeChange"
            :defaultValue="optionsPermissionTypeValue"
          />
        </el-form-item>
      </re-col>
      <re-col>
        <el-form-item label="上级节点">
          <el-cascader
            class="w-full"
            v-model="newFormInline.superiorId"
            :options="newFormInline.higherPermissionOptions"
            :props="{
              value: 'permissionId',
              label: 'label',
              emitPath: false,
              checkStrictly: true
            }"
            clearable
            filterable
            placeholder="请选择上级节点"
          >
            <template #default="{ node, data }">
              <span>{{ data.label }}</span>
              <span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
            </template>
          </el-cascader>
        </el-form-item>
      </re-col>

      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="权限名称" prop="label">
          <el-input
            v-model="newFormInline.label"
            clearable
            placeholder="请输入权限名称"
          />
        </el-form-item>
      </re-col>
      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="路径">
          <el-input
            v-model="newFormInline.path"
            clearable
            placeholder="请输入路径"
          />
        </el-form-item>
      </re-col>

      <re-col>
        <el-form-item label="请求方式" prop="httpMethods">
          <Segmented
            :options="optionsHttpMethods"
            @change="onHttpMethodsChange"
            :defaultValue="optionsHttpMethodsValue"
          />
        </el-form-item>
      </re-col>
      <re-col>
        <el-form-item label="排序">
          <el-input-number
            v-model="newFormInline.sort"
            :min="0"
            :max="9999"
            controls-position="right"
          />
        </el-form-item>
      </re-col>
      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="权限状态">
          <el-switch
            v-model="newFormInline.enabled"
            inline-prompt
            :active-value="true"
            :inactive-value="false"
            active-text="启用"
            inactive-text="停用"
            :style="switchStyle"
          />
        </el-form-item>
      </re-col>
      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="隐藏">
          <el-switch
            v-model="newFormInline.hidden"
            inline-prompt
            :active-value="false"
            :inactive-value="true"
            active-text="显示"
            inactive-text="隐藏"
            :style="switchStyle"
          />
        </el-form-item>
      </re-col>
      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="可关闭">
          <el-switch
            v-model="newFormInline.closable"
            inline-prompt
            :active-value="true"
            :inactive-value="false"
            active-text="是"
            inactive-text="否"
            :style="switchStyle"
          />
        </el-form-item>
      </re-col>
      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="可打开">
          <el-switch
            v-model="newFormInline.opened"
            inline-prompt
            :active-value="true"
            :inactive-value="false"
            active-text="是"
            inactive-text="否"
            :style="switchStyle"
          />
        </el-form-item>
      </re-col>
      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="打开新窗口">
          <el-switch
            v-model="newFormInline.newWindow"
            inline-prompt
            :active-value="true"
            :inactive-value="false"
            active-text="是"
            inactive-text="否"
            :style="switchStyle"
          />
        </el-form-item>
      </re-col>
      <re-col :value="12" :xs="24" :sm="24">
        <el-form-item label="链接外显">
          <el-switch
            v-model="newFormInline.external"
            inline-prompt
            :active-value="true"
            :inactive-value="false"
            active-text="是"
            inactive-text="否"
            :style="switchStyle"
          />
        </el-form-item>
      </re-col>

      <re-col>
        <el-form-item label="描述">
          <el-input
            v-model="newFormInline.description"
            placeholder="请输入描述"
          />
        </el-form-item>
      </re-col>
    </el-row>
  </el-form>
</template>
