 <template>
	<el-container>
		<el-header>
			<div class="left-panel">
				<el-button type="primary" icon="el-icon-plus" @click="add"></el-button>
				<el-button type="danger" plain icon="el-icon-delete" :disabled="selection.length==0" @click="batch_del"></el-button>
				<el-button type="primary" plain :disabled="selection.length!=1" @click="permission">权限设置</el-button>
			</div>
			<div class="right-panel">
				<div class="right-panel-search">
					<el-input v-model="search.roleName" placeholder="角色名称" clearable></el-input>
					<el-button type="primary" icon="el-icon-search" @click="upsearch"></el-button>
				</div>
			</div>
		</el-header>
		<el-main class="nopadding">
			<scTable ref="table" @current-change="handleCurrentPageChange" :page-size="pageSize"  @pagination-change="handlePaginationChange" @pagesize="pageSizeChange"  row-key="roleId" @selection-change="selectionChange" stripe>
				<el-table-column type="selection" width="50"></el-table-column>
				<el-table-column label="#" type="index" width="50"></el-table-column>
				<el-table-column label="角色名称" prop="roleName" width="150"></el-table-column>
				<el-table-column label="别名" prop="roleCode" width="200"></el-table-column>
				<el-table-column label="排序" prop="sort" width="80"></el-table-column>
				<el-table-column label="状态" prop="isActive" width="80">
					<template #default="scope">
						<el-switch v-model="scope.row.isActive" @change="changeSwitch($event, scope.row)" :loading="scope.row.$switch_status" :active-value="true" :inactive-value="false"></el-switch>
					</template>
				</el-table-column>
				<el-table-column label="创建时间" prop="created" width="180"></el-table-column>
				<el-table-column label="备注" prop="description" min-width="150"></el-table-column>
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

	<permission-dialog v-if="dialog.permission" ref="permissionDialog" @closed="dialog.permission=false"></permission-dialog>

</template>

<script>
	import saveDialog from './save'
	import permissionDialog from './permission'

	export default {
		name: 'role',
		components: {
			saveDialog,
			permissionDialog
		},
		data() {
			return {
				dialog: {
					save: false,
					permission: false
				},
				selection: [],
				search: {
					keyword: null,
					roleName:null
				},
                currentPage: 1, // 当前页数
                pageSize: 10, // 每页显示的条数
			}
		},
		 mounted() {
		    this.query();
		},
		methods: {
			async query(){
				let response =  await this.$API.system.role.list.post({
					roleName:this.search.roleName,
					pageNumber:this.currentPage,
					pageSize:this.pageSize,
					orderBy: "Id",
                    // "sortDirection": "",
					isActive:null
				})
				this.$refs.table.total = response.data.totalItems;
				this.$refs.table.tableData = response.data.items;
			},
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
			//权限设置
			permission(){
				this.dialog.permission = true
				this.$nextTick(() => {
					this.$refs.permissionDialog.open()
				})
			},
			//删除
			async table_del(row){
				var reqData = {"roleIds": [row.roleId]}
				var res = await this.$API.system.role.delete.delete(reqData);
				if(res.succeeded){
					this.$refs.table.refresh()
					this.$message.success("删除成功")				
					this.query();
				}else{
					this.$alert(res.error, "提示", {type: 'error'})
				}
			},
			//批量删除
			async batch_del(){
				this.$confirm(`确定删除选中的 ${this.selection.length} 项吗？如果删除项中含有子集将会被一并删除`, '提示', {
					type: 'warning'
				}).then(async () => {					
					const roleIds = this.selection.map(item => item.roleId);
					var reqData = {"roleIds": roleIds}
					var res = await this.$API.system.role.delete.delete(reqData);
					if(res.succeeded){
						const loading = this.$loading();
					    this.$refs.table.refresh()
					    loading.close();
					    this.$message.success("操作成功")
						this.query();
					}else{
					    this.$alert(res.error, "提示", {type: 'error'})
				    }					
				}).catch(() => {

				})
			},
			//表格选择后回调事件
			selectionChange(selection){
				this.selection = selection;
			},
			//表格内开关
			async changeSwitch(val, row){
				row.isActive = row.isActive === true?false:true
				row.$switch_status = true;
				var res = await this.$API.system.role.isActive.put({
					roleId:row.roleId,
					isActive:val,
					concurrencyStamp:row.concurrencyStamp});
				if(res.succeeded && res.data){
					delete row.$switch_status;
					row.isActive = val;
					this.$message.success("操作成功")
				} else {
					this.$message.error(res.error)
				}
				this.query();
			},
			//搜索
			upsearch(){
				this.query();
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
			//本地更新数据
			handleSaveSuccess(data, mode){
				this.query();
				if(mode=='add'){
					this.$refs.table.refresh()
				}else if(mode=='edit'){
					this.$refs.table.refresh()
				}
			},
			//点击分页
			handlePaginationChange(val){		
				this.currentPage = val;
				this.query();
			},
			//行改变
			handleCurrentPageChange() {			
            },
			pageSizeChange(size){
				this.pageSize = size;
				this.search.roleName
				this.query();
			}
		},
		watch:{
			'pageSize'(val){
				console.log("22222222", val)
			}
		}
	}
</script>

<style>
</style>
