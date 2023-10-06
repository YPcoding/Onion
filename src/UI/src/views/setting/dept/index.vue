<template>
	<el-container>
		<el-header>
			<div class="left-panel">
				<el-button type="primary" icon="el-icon-plus" @click="add"></el-button>
				<el-button type="danger" plain icon="el-icon-delete" :disabled="selection.length==0" @click="batch_del"></el-button>
			</div>
			<div class="right-panel">
				<div class="right-panel-search">
					<el-input v-model="search.keyword" placeholder="部门名称" clearable></el-input>
					<el-button type="primary" icon="el-icon-search" @click="upsearch"></el-button>
				</div>
			</div>
		</el-header>
		<el-main class="nopadding">
			<scTable ref="table" :data="treeData" row-key="id" @selection-change="selectionChange" hidePagination>
				<el-table-column type="selection" width="50"></el-table-column>
				<el-table-column label="部门名称" prop="departmentName" width="250"></el-table-column>
				<el-table-column label="排序" prop="sort" width="150"></el-table-column>
				<el-table-column label="状态" prop="isActive" width="150">
					<template #default="scope">
						<el-tag v-if="scope.row.isActive===true" type="success">启用</el-tag>
						<el-tag v-if="scope.row.isActive===false" type="danger">停用</el-tag>
					</template>
				</el-table-column>
				<el-table-column label="创建时间" prop="created" width="180"></el-table-column>
				<el-table-column label="备注" prop="description" min-width="300"></el-table-column>
				<el-table-column label="操作" fixed="right" align="right" width="170">
					<template #default="scope">
						<el-button-group>
							<el-button text type="primary" size="small" @click="table_show(scope.row, scope.$index)">查看</el-button>
							<el-button text type="primary" size="small" @click="table_edit(scope.row, scope.$index)">编辑</el-button>
							<el-popconfirm title="确定删除吗？" @confirm="table_del(scope.row, scope.$index)">
								<template #reference>
									<el-button text type="primary" size="small">删除</el-button>
								</template>
							</el-popconfirm>
						</el-button-group>
					</template>
				</el-table-column>

			</scTable>
		</el-main>
	</el-container>

	<save-dialog v-if="dialog.save" ref="saveDialog" @success="handleSaveSuccess" @closed="dialog.save=false"></save-dialog>

</template>

<script>
	import saveDialog from './save'

	export default {
		name: 'dept',
		components: {
			saveDialog
		},
		data() {
			return {
				dialog: {
					save: false
				},
				data: [],
				selection: [],
				search: {
					keyword: null
				},
				treeData:[]
			}
		},
		async mounted() {
			this.upsearch();
		},
		methods: {
			//添加
			add(){
				this.dialog.save = true
				this.$nextTick(() => {
					this.$refs.saveDialog.open()
				})
			},
			//编辑
			table_edit(row){
				this.dialog.save = true
				this.$nextTick(() => {
					this.$refs.saveDialog.open('edit').setData(row)
				})
			},
			//查看
			table_show(row){
				this.dialog.save = true
				this.$nextTick(() => {
					this.$refs.saveDialog.open('show').setData(row)
				})
			},
			//删除
			async table_del(row){			
				var reqData = {"departmentIds": [row.departmentId]}
				var res = await this.$API.system.dept.delete.delete(reqData);
				if(res.succeeded&&res.data){
					this.$refs.table.refresh()
					this.$message.success("删除成功")
					this.upsearch();
				}else{
					this.$alert(res.error, "提示", {type: 'error'})
				}
			},
			//批量删除
			async batch_del(){
				this.$confirm(`确定删除选中的 ${this.selection.length} 项吗？如果删除项中含有子集将会被一并删除`, '提示', {
					type: 'warning'
				}).then(() => {
					const loading = this.$loading();
					this.$refs.table.refresh()
					loading.close();
					this.$message.success("操作成功")
				}).catch(() => {

				})
			},
			//表格选择后回调事件
			selectionChange(selection){
				this.selection = selection;
			},
			//搜索
			async upsearch(){
				 var res = await this.$API.system.dept.list.post({"departmentName":this.search.keyword});
				 this.treeData = this.convertToElTreeData(res?.data);
			},
			//根据ID获取树结构
			filterTree(id){
				var target = null;
				function filter(tree){
					tree.forEach(item => {
						if(item.id == id){
							target = item
						}
						if(item.children){
							filter(item.children)
						}
					})
				}
				filter(this.$refs.table.tableData)
				return target
			},
			convertToElTreeData(data, parentId = null) {
                const treeData = [];
                for (const item of data) {
                    if ((item.superiorId === parentId) || (parentId === null && !item.superiorId)) {
                        const children = this.convertToElTreeData(data, item.id);
                        const treeNode = {
                            id: item.id,
							parentId: item.superiorId,
							sort:item.sort,
                            departmentName: item.departmentName,
							isActive: item.isActive,
							created: item.created,
							description: item.description,
							departmentId:item.departmentId,
							concurrencyStamp:item.concurrencyStamp,
                            children: children.length > 0 ? children : null,
                        };
                        treeData.push(treeNode);
                    }
                }
                return treeData;
            },
		},
		watch:{
			'dialog.save'(val){
				if (!val) {
					this.upsearch();
				}
			}
		}
	}
</script>

<style>
</style>
