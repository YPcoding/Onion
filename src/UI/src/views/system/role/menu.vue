<script setup lang="ts">
import { ref, onMounted } from "vue";
import type { ElTreeV2 } from "element-plus";
import { getRolePermissionsByRoleId } from "@/api/system/role";
import { MenuFormProps } from "./utils/types";
import { cloneDeep } from "@pureadmin/utils";

const props = withDefaults(defineProps<MenuFormProps>(), {
  formInline: () => ({
    roleId: 0,
    roleName: "",
    permissionOptions: [],
    permissionIds: [],
    rolePermissionsData: [],
    concurrencyStamp: ""
  })
});

const ruleFormRef = ref();
const newFormInline = ref(props.formInline);

function getRef() {
  return ruleFormRef.value;
}

const treeRef = ref<InstanceType<typeof ElTreeV2>>();
const treeData = ref([]); // 这里存放转换后的数据
const treeProps = {
  // 定义节点属性，根据你的数据结构来配置
  id: "id",
  label: "label",
  children: "children"
};
// 假设这里存放原始数据
const originalData = ref([]);
const defaultCheckedKeys = ref([]);
const defaultExpandedKeys = ref([]);

onMounted(async () => {
  // 在组件创建时进行数据转换
  originalData.value = (
    await getRolePermissionsByRoleId(newFormInline.value.roleId)
  ).data;
  const data = cloneDeep(originalData.value);
  defaultCheckedKeys.value = cloneDeep(
    data.filter(f => f.has === true && f.type === 30).map(m => m.id)
  );
  defaultExpandedKeys.value = cloneDeep(data.map(m => m.id));
  treeData.value = convertToTreeData(originalData.value);
});

const convertToTreeData = originalData => {
  // 创建一个映射对象，用于通过 id 快速查找节点
  const idToNodeMap = {};

  // 遍历原始数据，将节点映射到 idToNodeMap
  originalData.forEach(node => {
    const id = node.id;
    idToNodeMap[id] = { ...node, children: [] };
  });

  const treeData = [];

  // 再次遍历原始数据，构建树形结构
  originalData.forEach(node => {
    const parentId = node?.parentId ?? "0";

    if (parentId === "0") {
      // 根节点，直接添加到 treeData
      treeData.push(idToNodeMap[node.id]);
    } else {
      // 非根节点，将其添加到父节点的 children 数组中
      if (idToNodeMap[parentId]) {
        idToNodeMap[parentId].children.push(idToNodeMap[node.id]);
      }
    }
  });

  return treeData;
};

function handleCheck() {
  getCheckedNodes();
}
const getCheckedNodes = () => {
  const halfCheckedNodes = treeRef.value.getHalfCheckedNodes();
  const checkedNodes = treeRef.value.getCheckedNodes();
  const allNodes = halfCheckedNodes.concat(checkedNodes);
  const nodes = cloneDeep(allNodes);
  newFormInline.value.permissionIds = nodes.map(item => item.permissionId);
};
</script>
<template>
  <div>
    <el-tree-v2
      ref="treeRef"
      :data="treeData"
      :props="treeProps"
      show-checkbox
      @check="handleCheck"
      :default-checked-keys="defaultCheckedKeys"
      :default-expanded-keys="defaultExpandedKeys"
    ></el-tree-v2>
  </div>
</template>
