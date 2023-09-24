<script setup lang="ts">
import { ref, onMounted } from "vue";

const treeData = ref([]); // 这里存放转换后的数据
const treeProps = {
  // 定义节点属性，根据你的数据结构来配置
  id: "id",
  label: "label",
  children: "children"
};
// 假设这里存放原始数据
const originalData = ref([
  {
    id: "4226506766234325007",
    permissionId: "4226506766234325007",
    superiorId: "4226475200187633674",
    parentId: "4226475200187633674",
    label: "获取角色权限",
    has: false
  },
  {
    id: "4226475200183439360",
    permissionId: "4226475200183439360",
    superiorId: "0",
    parentId: "0",
    label: "系统管理",
    has: false
  },
  {
    id: "4226475200187633664",
    permissionId: "4226475200187633664",
    superiorId: "4226475200183439360",
    parentId: "4226475200183439360",
    label: "授权管理",
    has: false
  }
]);

onMounted(() => {
  // 在组件创建时进行数据转换
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
    const parentId = node.parentId;

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
</script>
<template>
  <div>
    <el-tree-v2 :data="treeData" :props="treeProps" show-checkbox></el-tree-v2>
  </div>
</template>
