<template>
	<el-container>
		<el-aside width="200px" v-loading="showGrouploading">
			<el-container>
				<el-header>
					<el-input placeholder="输入关键字进行过滤" v-model="groupFilterText" clearable></el-input>
				</el-header>
				<el-main class="nopadding">
					<el-tree ref="group" class="menu" node-key="id" :data="group" :current-node-key="''" :highlight-current="true" :expand-on-click-node="false" :filter-node-method="groupFilterNode" @node-click="groupClick"></el-tree>
				</el-main>
			</el-container>
		</el-aside>
		<el-container>
				<el-header>
					<div class="left-panel">
						<el-button type="primary" icon="el-icon-plus" @click="add"></el-button>
						<el-button type="danger" plain icon="el-icon-delete" :disabled="selection.length==0" @click="batch_del"></el-button>
						<el-button type="primary" plain :disabled="selection.length==0">分配角色</el-button>
						<el-button type="primary" plain :disabled="selection.length==0">密码重置</el-button>
					</div>
					<div class="right-panel">
						<div class="right-panel-search">
							<el-input v-model="search.keyword" placeholder="登录账号 / 姓名" clearable></el-input>
							<el-button type="primary" icon="el-icon-search" @click="upsearch"></el-button>
						</div>
					</div>
				</el-header>
				<el-main class="nopadding">
					<scTable ref="table" :data="users" @selection-change="selectionChange" @pagination-change="handlePaginationChange" @pagesize="pageSizeChange" stripe remoteSort remoteFilter>
						<el-table-column type="selection" width="50"></el-table-column>
						<el-table-column label="ID" prop="userId" width="80" sortable="custom">
                            <template #default="scope">
                                <el-tooltip class="box-item" effect="dark" :content="scope.row.userId" placement="top-start">
                                    <span>{{ typeof scope.row.userId === 'string' ? scope.row.userId.slice(-5) : scope.row.userId }}</span>
                                </el-tooltip>
                            </template>
                        </el-table-column>
						<el-table-column label="头像" width="80" column-key="filterProfilePictureDataUrl" :filters="[{text: '已上传', value: '1'}, {text: '未上传', value: '0'}]">
							<template #default="scope">
								<el-avatar :src="scope.row.profilePictureDataUrl" size="small"></el-avatar>
							</template>
						</el-table-column>
						<el-table-column label="登录账号" prop="userName" width="150" sortable='custom' column-key="filterUserName" :filters="[{text: '系统账号', value: '1'}, {text: '普通账号', value: '0'}]"></el-table-column>
						<el-table-column label="姓名" prop="realname" width="150" sortable='custom'></el-table-column>
						<el-table-column label="所属角色" prop="roles">
                            <template #default="scope">
								<el-tag v-for="(item, index) in scope.row.roles" :key="index">{{ item.roleName }}</el-tag>
                            </template>
                        </el-table-column>
						<el-table-column label="加入时间" prop="created" width="170" sortable='custom'></el-table-column>
						<el-table-column label="操作" fixed="right" align="right" width="160">
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
	</el-container>

	<save-dialog v-if="dialog.save" ref="saveDialog" @success="handleSuccess" @closed="dialog.save=false"></save-dialog>

</template>

<script>
	import saveDialog from './save'

	export default {
		name: 'user',
		components: {
			saveDialog
		},
		data() {
			return {
				dialog: {
					save: false
				},
				showGrouploading: false,
				groupFilterText: '',
				group: [],
				users: [],
				selection: [],
				search: {
					keyword: null,
					departmentId:null
				},
				currentPage: 1, // 当前页数
                pageSize: 10, // 每页显示的条数
			}
		},
		mounted() {
			this.getGroup();
			this.query();
		},
		methods: {
			//添加
			add(){
				this.dialog.save = true
				this.$nextTick(() => {
					this.$refs.saveDialog.open('add')
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
				var reqData = {"userIds": [row.userId]}
				var res = await this.$API.system.user.delete.delete(reqData);
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
				this.$confirm(`确定删除选中的 ${this.selection.length} 项吗？`, '提示', {
					type: 'warning'
				}).then(async () => {
					const loading = this.$loading();
					const userIds = this.selection.map(item => item.userId);
					var reqData = {"userIds": userIds}
					var res = await this.$API.system.user.delete.delete(reqData);
					if(res.succeeded){
						const loading = this.$loading();
					    this.$refs.table.refresh()
					    loading.close();
					    this.$message.success("操作成功")
						this.query();
					}else{
					    this.$alert(res.error, "提示", {type: 'error'})
				    }
					loading.close();
				}).catch(() => {

				})
			},
			//表格选择后回调事件
			selectionChange(selection){
				this.selection = selection;
			},
			//加载树数据
			async getGroup(){
				this.showGrouploading = true;
				var res = await this.$API.system.dept.list.post();
				this.treeData = this.convertToElTreeData(res?.data);
				this.showGrouploading = false;
				var allNode ={id: '', label: '所有'}
				this.treeData.unshift(allNode);
				console.log(this.treeData)
				this.group = this.treeData;
			},
			//树过滤
			groupFilterNode(value, data){
				if (!value) return true;
				return data.label.indexOf(value) !== -1;
			},
			//树点击事件
			groupClick(data){			
				var params = {
					groupId: data.id
				}
				this.$refs.table.reload(params)
				this.search.departmentId = data.id==""?null:data.id
				this.query();
			},
			//搜索
			upsearch(){
				//this.$refs.table.upData(this.search)
				 this.query();
			},
			async query(){
				let response =  await this.$API.system.user.list.post({
					pageNumber:this.currentPage,
					pageSize:this.pageSize,
					orderBy: "Id",
                    // "sortDirection": "",
					keyword:this.search.keyword,
					departmentId:this.search.departmentId,
				})
				this.$refs.table.total = response.data.totalItems;
				this.$refs.table.tableData = response.data.items;
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
				console.log("111",size )
				this.pageSize = size;
				this.search.keyword
				this.query();
			},
			//本地更新数据
			handleSuccess(data, mode){
				if(mode=='add'){
					data.id = new Date().getTime()
					this.$refs.table.tableData.unshift(data)
				}else if(mode=='edit'){
					this.$refs.table.tableData.filter(item => item.id===data.id ).forEach(item => {
						Object.assign(item, data)
					})
				}
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
							label: item.departmentName,
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
            }
		},
		watch: {
			groupFilterText(val) {
				this.$refs.group.filter(val);
			},
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
