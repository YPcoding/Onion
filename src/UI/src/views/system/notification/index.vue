﻿
<script setup lang="ts">
import { ref } from "vue";
import { useNotification } from "./utils/hook";
import { useRenderIcon } from "@/components/ReIcon/src/hooks";
import { PureTableBar } from "@/components/RePureTableBar";
import { hasAuth } from "@/router/utils";

import Search from "@iconify-icons/ep/search";
import Delete from "@iconify-icons/ep/delete";
import EditPen from "@iconify-icons/ep/edit-pen";
import Refresh from "@iconify-icons/ep/refresh";
import AddFill from "@iconify-icons/ri/add-circle-line";

defineOptions({
  name: "Notification"
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
} = useNotification(tableRef, treeRef);
</script>
<template>
  <div
    class="flex justify-between"
    v-if="hasAuth('api:notification:paginationquery')"
  >

                        
    <div class="w-[calc(100%-0px)]">
      <el-form
        ref="formRef"
        :inline="true"
        :model="form"
        class="search-form bg-bg_color w-[99/100] pl-8 pt-[12px]"
      >

                        
        <el-form-item label="通知标题：" prop="title">
          <el-input
            v-model="form.title"
            placeholder="请输入通知标题"
            clearable
            class="!w-[160px]"
          />
        </el-form-item>

        <el-form-item label="通知内容：" prop="content">
          <el-input
            v-model="form.content"
            placeholder="请输入通知内容"
            clearable
            class="!w-[160px]"
          />
        </el-form-item>

        <el-form-item label="相关链接：" prop="link">
          <el-input
            v-model="form.link"
            placeholder="请输入相关链接"
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

      <PureTableBar title="通知管理" :columns="columns" @refresh="onSearch">
        <template #buttons>
          <el-button
            type="primary"
            :icon="useRenderIcon(AddFill)"
            @click="openDialog()"
            v-if="hasAuth('api:notification:add')"
          >
            新增
          </el-button>
        </template>
        <template v-slot="{ size, dynamicColumns }">
          <div
            v-if="selectedNum > 0"
            v-motion-fade
            class="bg-[var(--el-fill-color-light)] w-full h-[46px] mb-2 pl-4 flex items-center"
          >
            <div class="flex-auto">
              <span
                style="font-size: var(--el-font-size-base)"
                class="text-[rgba(42,46,54,0.5)] dark:text-[rgba(220,220,242,0.5)]"
              >
                已选 {{ selectedNum }} 项
              </span>
              <el-button type="primary" text @click="onSelectionCancel">
                取消选择
              </el-button>
            </div>
            <el-popconfirm title="是否确认删除?" @confirm="onbatchDel">
              <template #reference>
                <el-button
                  type="danger"
                  text
                  class="mr-1"
                  v-if="hasAuth('api:notification:delete')"
                >
                  批量删除
                </el-button>
              </template>
            </el-popconfirm>
          </div>
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
            <template #operation="{ row }">
              <el-button
                class="reset-margin"
                link
                type="primary"
                :size="size"
                :icon="useRenderIcon(EditPen)"
                @click="openDialog('编辑', row)"
                v-if="hasAuth('api:notification:update')"
              >
                修改
              </el-button>
              <el-popconfirm
                :title="`是否确认删除编号为${row.notificationId}的这条数据`"
                @confirm="handleDelete(row)"
              >
                <template #reference>
                  <el-button
                    class="reset-margin"
                    link
                    type="primary"
                    :size="size"
                    :icon="useRenderIcon(Delete)"
                    v-if="hasAuth('api:notification:delete')"
                  >
                    删除
                  </el-button>
                </template>
              </el-popconfirm>
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
