<template>
	<el-main>
		<el-row :gutter="15">
			<el-col :xl="6" :lg="6" :md="8" :sm="12" :xs="24" v-for="item in list" :key="item.id">
				<el-card class="task task-item" shadow="hover">
					<h2>{{item.jobName}}</h2>
					<ul>
						<li>
							<h4>执行类</h4>
							<p>{{item.jobGroup}}</p>
						</li>
						<li>
							<h4>定时规则</h4>
							<p>{{item.cronExpression}}</p>
						</li>
					</ul>
					<div class="bottom">
						<div class="state">
							<el-tag v-if="item.status=='Normal'" size="small" >正在执行</el-tag>
							<el-tag v-if="item.status=='Paused'" size="small" type="danger">暂停</el-tag>
							<el-tag v-if="item.status=='Completed'" size="small" type="success">完成</el-tag>
							<el-tag v-if="item.status=='Error'" size="small" type="danger">执行失败</el-tag>
							<el-tag v-if="item.status=='Blocked'" size="small" type="info">阻塞</el-tag>
							<el-tag v-if="item.status=='None'" size="small" type="info">无此任务</el-tag>
						</div>
						<div class="handler">
							<el-popconfirm :title="item.status=='Normal'?'确定要暂停正在执行的任务吗？':'确定立即执行吗？'" @confirm="run(item)">
								<template #reference>						
									<el-button type="primary" icon="el-icon-caret-right" circle></el-button>
								</template>
							</el-popconfirm>
							<el-dropdown trigger="click">
								<el-button type="primary" icon="el-icon-more" circle plain></el-button>
								<template #dropdown>
									<el-dropdown-menu>
										<el-dropdown-item @click="edit(item)">编辑</el-dropdown-item>
										<el-dropdown-item @click="logs(item)">日志</el-dropdown-item>
										<el-dropdown-item @click="del(item)" divided>删除</el-dropdown-item>
									</el-dropdown-menu>
								</template>
							</el-dropdown>
						</div>
					</div>
				</el-card>
			</el-col>
			<el-col :xl="6" :lg="6" :md="8" :sm="12" :xs="24">
				<el-card class="task task-add" shadow="never" @click="add">
					<el-icon><el-icon-plus /></el-icon>
					<p>添加计划任务</p>
				</el-card>
			</el-col>
		</el-row>
	</el-main>

	<save-dialog v-if="dialog.save" ref="saveDialog" @success="handleSuccess" @closed="dialog.save=false"></save-dialog>

	<el-drawer title="计划任务日志" v-model="dialog.logsVisible" :size="600" direction="rtl" destroy-on-close>
		<logs></logs>
	</el-drawer>
</template>

<script>
	import saveDialog from './save'
	import logs from './logs'
	import hubConnection, { startSignalRConnection } from "@/api/signalRConnection";


	export default {
		name: 'task',
		components: {
			saveDialog,
			logs
		},
		provide() {
			return {
				list: this.list
			}
		},
		data() {
			return {
				dialog: {
					save: false,
					logsVisible: false
				},
				list: [],
				icon:"el-icon-video-pause" //el-icon-caret-right
			}
		},
		mounted() {
			this.query();
			hubConnection.on("ReceivePublicMessage", (user, message) => {
                console.log(`Received message from ${user}: ${message}`);
				this.query();
            });
		},
		methods: {
			async query(){
				var response = await this.$API.system.tasks.list.post({});
			    if (response?.succeeded) {
					this.list=response.data.items
			    } 
			},
			add(){
				this.dialog.save = true
				this.$nextTick(() => {
					this.$refs.saveDialog.open()
				})
			},
			edit(task){
				this.dialog.save = true
				this.$nextTick(() => {
					this.$refs.saveDialog.open('edit').setData(task)
				})
			},
			del(task){
				this.$confirm(`确认删除 ${task.jobName} 计划任务吗？`,'提示', {
					type: 'warning',
					confirmButtonText: '删除',
					confirmButtonClass: 'el-button--danger'
				}).then(async () => {
			  		var response = await this.$API.system.tasks.delete.delete({scheduledJobIds:[task.scheduledJobId]});
			        if (response?.succeeded) {
				        this.JobGroupOptions=response.data
						this.query();
			        } 
					this.list.splice(this.list.findIndex(item => item.id === task.id), 1)
				}).catch(() => {
					//取消
				})
			},
			async logs(task){
				console.log(task)
				var response = await this.$API.system.tasks.logList.post({
					pageNumber:1,
					pageSize:10,
					orderBy: "Id",
					sortDirection: "Descending",
					jobGroup:task.jobGroup,
					jobName:task.jobName,
				});
			    if (response?.succeeded) {
					// const logList = [];
				    // for (const item of response.data.items) 
				    // {
					// 	let data = JSON.parse(item.properties);
					//     data.level = item.level
					//     data.exception = item.exception
					//     logList.push(data);
				    // }
				    // this.$refs.table.total = response.data.totalItems;
				    // this.dataList = logList
					// this.list = response.data.items
			    } 
				
				this.dialog.logsVisible = true
			},
			async run(task){
				if (task.status === 'Normal') {
					task.status ='Paused';
				}
				else{
					task.status ='Normal';
				}
				var response = await this.$API.system.tasks.updateJobStatus.put(
				{
					scheduledJobId:task.scheduledJobId,
					concurrencyStamp:task.concurrencyStamp,
					status:task.status,
				});
			    if (response?.succeeded) {
					this.query();
					if (task.status === 'Normal') {
						this.$message.success(`已成功执行计划任务：${task.jobName}`)
					} else {
						this.$message.success(`已成功暂停计划任务：${task.jobName}`)
					}	 
			    } else{
					this.$message.error(`执行失败：${response.message}`)
				}

			},
			//本地更新数据
			handleSuccess(data, mode){
				if(mode=='add'){
					data.id = new Date().getTime()
					this.list.push(data)
				}else if(mode=='edit'){
					this.list.filter(item => item.id===data.id ).forEach(item => {
						Object.assign(item, data)
					})
				}
			}
		},
		watch: {
			groupFilterText(val) {
				this.$refs.group.filter(val);
			},
			'dialog.save'(val){
				if (!val) {
					this.query();
				}
			}
		},
        async created() {
	        if (hubConnection.state === "Disconnected") {
                startSignalRConnection().then(isConnected => {
                    if (isConnected) {
                        // 连接成功，可以监听事件或发送消息
                    }
                });
            } else {
                // 已连接，可以监听事件或发送消息
            }
        }
    }
</script>

<style scoped>
	.task {height: 210px;}
	.task-item h2 {font-size: 15px;color: #3c4a54;padding-bottom:15px;}
	.task-item li {list-style-type:none;margin-bottom: 10px;}
	.task-item li h4 {font-size: 12px;font-weight: normal;color: #999;}
	.task-item li p {margin-top: 5px;}
	.task-item .bottom {border-top: 1px solid #EBEEF5;text-align: right;padding-top:10px;display: flex;justify-content: space-between;align-items: center;}

	.task-add {display: flex;flex-direction: column;align-items: center;justify-content: center;text-align: center;cursor: pointer;color: #999;}
	.task-add:hover {color: #409EFF;}
	.task-add i {font-size: 30px;}
	.task-add p {font-size: 12px;margin-top: 20px;}
	
	.dark .task-item .bottom {border-color: var(--el-border-color-light);}
</style>
