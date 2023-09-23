<script setup lang="ts">
import { ref } from "vue";
import ReCol from "@/components/ReCol";
import { RoleFormProps } from "../utils/types";

const props = withDefaults(defineProps<RoleFormProps>(), {
  formInline: () => ({
    userId: "",
    userName: "",
    roleOptions: [],
    roleIds: []
  })
});

const newFormInline = ref(props.formInline);
</script>

<template>
  <el-form :model="newFormInline">
    <el-row :gutter="30">
      <re-col>
        <el-form-item label="用户名称" prop="userName">
          <el-input disabled v-model="newFormInline.userName" />
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
    </el-row>
  </el-form>
</template>
