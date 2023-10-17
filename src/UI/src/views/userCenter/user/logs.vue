<template>
	<el-card shadow="never" header="近7天操作记录">
		<scTable ref="table" :page-size="pageSize" @pagination-change="handlePaginationChange" @pagesize="pageSizeChange"
			height="auto" paginationLayout="total, prev, pager, next" hideDo>
			<sc-table-column label="序号" type="index"></sc-table-column>
			<el-table-column label="级别" prop="level" width="50">
				<template #default="scope">
					<el-icon v-if="scope.row.level == 'Error'"
						style="color: #F56C6C;"><el-icon-circle-close-filled /></el-icon>
					<el-icon v-if="scope.row.level == 'Warning'"
						style="color: #E6A23C;"><el-icon-warning-filled /></el-icon>
					<el-icon v-if="scope.row.level == 'Information'"
						style="color: #409EFF;"><el-icon-info-filled /></el-icon>
				</template>
			</el-table-column>
			<sc-table-column label="业务名称" prop="LoggerName" min-width="200"></sc-table-column>
			<sc-table-column label="IP" prop="ClientIP" width="150"></sc-table-column>
			<sc-table-column label="响应码" prop="ResponseStatusCode" width="150">
				<el-tag type="success">成功</el-tag>
			</sc-table-column>
			<sc-table-column label="操作结果" prop="Message" width="150"></sc-table-column>
			<sc-table-column label="操作时间" prop="LoggerTime" width="150"></sc-table-column>
		</scTable>
	</el-card>
</template>

<script>
export default {
	data () {
		return {
			currentPage: 1, // 当前页数
			pageSize: 10, // 每页显示的条数
		}
	},
	async mounted () {
		this.query()
	},
	methods: {
		async query () {
			let response = await await this.$API.system.log.list.post({
				pageNumber: this.currentPage,
				pageSize: this.pageSize,
				orderBy: "Id",
				sortDirection: "Descending",
				//keyword:this.search.keyword
			})

			const list = []
			for (const item of response.data.items) {
				let data = JSON.parse(item.properties)
				data.level = item.level
				list.push(data)
			}

			this.$refs.table.total = response.data.totalItems
			this.$refs.table.tableData = list
		},//点击分页
		handlePaginationChange (val) {
			this.currentPage = val
			this.query()
		},
		pageSizeChange (size) {
			this.pageSize = size
			this.query()
		}
	},
}
</script>

<style></style>
