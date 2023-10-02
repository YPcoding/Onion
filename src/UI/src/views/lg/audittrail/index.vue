
<script setup lang="ts">
import { ref } from "vue";
import { useAuditTrail } from "./utils/hook";
import { useRenderIcon } from "@/components/ReIcon/src/hooks";
import { PureTableBar } from "@/components/RePureTableBar";
import { hasAuth } from "@/router/utils";

import Search from "@iconify-icons/ep/search";
import Delete from "@iconify-icons/ep/delete";
import EditPen from "@iconify-icons/ep/edit-pen";
import Refresh from "@iconify-icons/ep/refresh";
import AddFill from "@iconify-icons/ri/add-circle-line";

defineOptions({
  name: "AuditTrail"
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
  handleDelete,
  handleSizeChange,
  onSelectionCancel,
  handleCurrentChange,
  handleSelectionChange
} = useAuditTrail(tableRef, treeRef);
</script>
<template>
  <div
    class="flex justify-between"
    v-if="hasAuth('api:audittrail:paginationquery')"
  >

                        
    <div class="w-[calc(100%-0px)]">
      <el-form
        ref="formRef"
        :inline="true"
        :model="form"
        class="search-form bg-bg_color w-[99/100] pl-8 pt-[12px]"
      >
                        
        <el-form-item label="表名：" prop="tableName">
          <el-input
            v-model="form.tableName"
            placeholder="请输入表名"
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

      <PureTableBar title="审计日志管理" :columns="columns" @refresh="onSearch">
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
          <p class="mb-2">编号：{{row.id}}</p>
          <p class="mb-2">主键：{{ JSON.parse(row.primaryKeyJson)}}</p>
          <p class="mb-2">旧值：{{ JSON.parse(row.oldValuesJson)}}</p>
          <p class="mb-2">新值：{{ JSON.parse(row.newValuesJson)}}</p>
          <p class="mb-2">受影响的列：{{JSON.parse(row.affectedColumnsJson)}}</p>
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
