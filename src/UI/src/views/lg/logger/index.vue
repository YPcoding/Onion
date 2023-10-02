
<script setup lang="ts">
import { ref } from "vue";
import { useLogger } from "./utils/hook";
import { useRenderIcon } from "@/components/ReIcon/src/hooks";
import { PureTableBar } from "@/components/RePureTableBar";
import { hasAuth } from "@/router/utils";

import Search from "@iconify-icons/ep/search";
import Delete from "@iconify-icons/ep/delete";
import EditPen from "@iconify-icons/ep/edit-pen";
import Refresh from "@iconify-icons/ep/refresh";
import AddFill from "@iconify-icons/ri/add-circle-line";

defineOptions({
  name: "Logger"
});
const treeRef = ref();
const formRef = ref();
const tableRef = ref();
const {
  form,
  loading,
  columns,
  dataList,
  pagination,
  selectedNum,
  onSearch,
  resetForm,
  onbatchDel,
  openDialog,
  handleDelete,
  handleSizeChange,
  onSelectionCancel,
  handleCurrentChange,
  handleSelectionChange
} = useLogger(tableRef, treeRef);
</script>
<template>
  <div
    class="flex justify-between"
    v-if="hasAuth('api:logger:paginationquery')"
  >

                        
    <div class="w-[calc(100%-0px)]">
      <el-form
        ref="formRef"
        :inline="true"
        :model="form"
        class="search-form bg-bg_color w-[99/100] pl-8 pt-[12px]"
      >



        <el-form-item label="消息等级：" prop="level">
          <el-input
            v-model="form.level"
            placeholder="请输入消息等级"
            clearable
            class="!w-[160px]"
          />
        </el-form-item>


        <el-form-item label="用户名：" prop="userName">
          <el-input
            v-model="form.userName"
            placeholder="请输入用户名"
            clearable
            class="!w-[160px]"
          />
        </el-form-item>

        <el-form-item label="客户端IP：" prop="clientIP">
          <el-input
            v-model="form.clientIP"
            placeholder="请输入客户端IP"
            clearable
            class="!w-[160px]"
          />
        </el-form-item>

        <el-form-item label="IP：" prop="clientAgent">
          <el-input
            v-model="form.clientAgent"
            placeholder="请输入IP"
            clearable
            class="!w-[160px]"
          />
        </el-form-item>


                        
        <el-form-item>
          <el-button :icon="useRenderIcon(Search)" type="primary" @click="onSearch"> 搜索 </el-button>
          <el-button :icon="useRenderIcon(Refresh)" @click="resetForm(formRef)">
            重置
          </el-button>
        </el-form-item>
      </el-form>

      <PureTableBar title="日志管理" :columns="columns" @refresh="onSearch">
        <template v-slot="{ size, dynamicColumns }">
          <pure-table
            row-key="id"
            ref="tableRef"
            adaptive
            align-whole="center"
            table-layout="auto"
            :loading="loading"
            :size="size"
            :data="dataList"
            :columns="dynamicColumns"
            :pagination="pagination"
            :paginationSmall="size === 'small' ? true : false"
            :header-cell-style="{
              background: 'var(--el-fill-color-light)',
              color: 'var(--el-text-color-primary)'
            }"
            @selection-change="handleSelectionChange"
            @page-size-change="handleSizeChange"
            @page-current-change="handleCurrentChange"
          >
          <template #expand="{ row }">
        <div class="m-4">     
          <p class="mb-2">编号：{{ row.id}}</p>
          <p class="mb-2">日志事件：{{ row.logEvent}}</p>
        </div>
      </template>
          </pure-table>
        </template>
      </PureTableBar>
    </div>
  </div>
</template>

<style scoped lang="scss">
:deep(.el-dropdown-menu__item i) {
  margin: 0;
}

:deep(.el-button:focus-visible) {
  outline: none;
}

.search-form {
  :deep(.el-form-item) {
    margin-bottom: 12px;
  }
}
</style>
