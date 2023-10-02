<script setup lang="ts">
import { type CSSProperties, ref, computed, onMounted } from "vue";
import { hasAuth } from "@/router/utils";
import {
  getGenerateCodeInfo,
  generateBackendCode,
  generateFrontendCode
} from "@/api/genrate/code";

defineOptions({
  name: "Genrate"
});

const elStyle = computed((): CSSProperties => {
  return {
    width: "85vw",
    justifyContent: "start"
  };
});

const entityOptions = ref([]);
const backendSavePath = ref("");
const frontendSavePath = ref("");
const selectedEntityOption = ref("");
const namespaceName = ref("");
const loading = ref(true);

//生命周期钩子函数
onMounted(async () => {
    onSearch();
});

//查询
async function onSearch() {
  const { data } = await getGenerateCodeInfo();
  entityOptions.value = buildOptions(data.entities);
  namespaceName.value = data.namespaceName;
  backendSavePath.value = data.backendSavePath;
  frontendSavePath.value = data.frontendSavePath;

  setTimeout(() => {
    loading.value = false;
  }, 500);
}
async function handleGenrate() {
  if (
    backendSavePath.value !== "" &&
    namespaceName.value !== "" &&
    selectedEntityOption.value !== "" &&
    hasAuth('api:code:backend')
  ) {
    var data = (
      await generateBackendCode({
        fullClassName: selectedEntityOption.value,
        nameSpace: namespaceName.value,
        savePath: backendSavePath.value,
        type: 0
      })
    ).data;
    console.log(data);
  }
  if (frontendSavePath.value !== "" && selectedEntityOption.value !== ""&&hasAuth('api:code:frontend')) {
    var data = (
      await generateFrontendCode({
        fullClassName: selectedEntityOption.value,
        savePath: frontendSavePath.value,
        type: 0
      })
    ).data;
    console.log(data);
  }
}

function buildOptions(data) {
  if (!data || !data.length) return;
  const options = [
    {
      value: "",
      label: ""
    }
  ];
  for (const item of data) {
    if (item !== "") {
      const option = { value: item, label: item };
      options.push(option);
    }
  }
  return options;
}
</script>

<template>
  <el-space direction="vertical" size="large" v-if="hasAuth('api:code:query:save:path')||hasAuth('api:code:backend')||hasAuth('api:code:frontend')">
    <el-tag :style="elStyle" size="large" effect="dark"> 代码生成器 </el-tag>
    <el-card shadow="never" :style="elStyle">
      <span>默认命名空间：</span>
      <el-input
        v-model="namespaceName"
        placeholder="请输入默认命名空间："
        clearable
        class="!w-[480px]"
      />
    </el-card>
    <el-card shadow="never" :style="elStyle">
      <span>选择生成的实体类：</span>
      <el-select
        v-model="selectedEntityOption"
        placeholder="请输入保存后端代码路径"
      >
        <el-option
          v-for="item in entityOptions"
          :key="item"
          :label="item.label"
          :value="item.value"
        />
      </el-select>
    </el-card>
    <el-card shadow="never" :style="elStyle" v-if="hasAuth('api:code:backend')">
      <span>选择保存后端代码路径：</span>
      <el-input
        v-model="backendSavePath"
        placeholder="请输入保存后端代码路径"
        clearable
        class="!w-[480px]"
      />
    </el-card>
    <el-card shadow="never" :style="elStyle" v-if="hasAuth('api:code:frontend')">
      <span>选择保存前端代码路径：</span>
      <el-input
        v-model="frontendSavePath"
        placeholder="请输入保存前端代码路径"
        clearable
        class="!w-[480px]"
      />
    </el-card>
    <el-button type="primary" @click="handleGenrate" v-if="hasAuth('api:code:backend')||hasAuth('api:code:frontend')"> 生成代码 </el-button>
  </el-space>
</template>
