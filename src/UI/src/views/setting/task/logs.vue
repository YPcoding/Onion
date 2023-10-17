<!--
 * @Descripttion: 系统计划任务配置
 * @version: 1.0
 * @Author: sakuya
 * @Date: 2021年7月7日09:28:32
 * @LastEditors:
 * @LastEditTime:
-->

<template>
	<el-container>
		<el-main style="padding:0 20px;">
			<scTable ref="table" :data="data" :page-size="pageSize" @pagination-change="handlePaginationChange"
				@pagesize="pageSizeChange" stripe>
				<el-table-column label="执行时间" prop="LoggerTime" width="200"></el-table-column>
				<el-table-column label="执行结果" prop="LastExecutionStatus" width="100">
					<template #default="scope">
						<span v-if="scope.row.LastExecutionStatus == 'Success'"
							style="color: #67C23A;"><el-icon><el-icon-success-filled /></el-icon></span>
						<span v-else style="color: #F56C6C;"><el-icon><el-icon-circle-close-filled /></el-icon></span>
					</template>
				</el-table-column>
				<el-table-column label="执行日志" prop="logs" width="100" fixed="right">
					<template #default="scope">
						<el-button size="small" @click="show(scope.row)" type="text">日志</el-button>
					</template>
				</el-table-column>
			</scTable>
		</el-main>
	</el-container>

	<el-drawer title="日志" v-model="logsVisible" :size="500" direction="rtl" destroy-on-close>
		<el-main style="padding:0 20px 20px 20px;">
			<pre
				style="font-size: 12px;color: #999;padding:20px;background: #333;font-family: consolas;line-height: 1.5;overflow: auto;">{{ demoLog }}</pre>
		</el-main>
	</el-drawer>
</template>

<script>
export default {
	data () {
		return {
			currentPage: 1, // 当前页数
			pageSize: 10, // 每页显示的条数
			logsVisible: false,
			demoLog: "",
			data: [],
			jobName: "",
			jobGroup: ""
		}
	},
	mounted () {
	},
	methods: {
		//显示
		open (jobName, jobGroup) {
			this.visible = true
			this.jobName = jobName
			this.jobGroup = jobGroup
			this.query()
			return this
		},
		async query () {
			var response = await this.$API.system.tasks.logList.post({
				pageNumber: this.currentPage,
				pageSize: this.pageSize,
				orderBy: "Id",
				sortDirection: "Descending",
				jobGroup: this.jobGroup,
				jobName: this.jobName,
			})
			if (response?.succeeded) {
				const logList = []
				for (const item of response.data.items) {
					let data = JSON.parse(item.properties)
					data.level = item.level
					data.exception = item.exception
					logList.push(data)
				}
				this.$refs.table.total = response.data.totalItems
				this.data = logList
				console.log("分页数据：", this.data)
			}
		},
		show (task) {
			this.demoLog = ""
			this.demoLog += `应用名称：${task.Application}\r\n`
			this.demoLog += `任务键值：${task.JobKey}\r\n`
			this.demoLog += `执行类名称：${task.SourceContext}\r\n`
			this.demoLog += `执行信息：${task.LastExecutionMessage}\r\n`
			this.demoLog += `上次执行时间：${task.LastExecutionTime}\r\n`
			this.demoLog += `下次执行时间：${task.NextExecutionTime}\r\n`
			this.demoLog += `执行异常信息：${task.exception}\r\n`
			this.demoLog += `日志等级：${task.level}\r\n`
			this.logsVisible = true
		},
		//点击分页
		handlePaginationChange (val) {
			this.currentPage = val
			this.query()
		},
		pageSizeChange (size) {
			this.pageSize = size
			this.query()
		}
	}
}
</script>

<style></style>
