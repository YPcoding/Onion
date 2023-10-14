<template>
	<el-container>
		<el-aside width="220px">
			<el-tree ref="category" @node-click="categoryClick" class="menu" node-key="label" :data="category" :default-expanded-keys="['系统日志']" current-node-key="系统日志" :highlight-current="true" :expand-on-click-node="false">
			</el-tree>
		</el-aside>
		<el-container>
			<el-main class="nopadding">
				<el-container>
					<el-header>
						<div class="left-panel">
							<el-date-picker v-model="date"  @change="handlePick" type="datetimerange" range-separator="至" start-placeholder="开始日期" end-placeholder="结束日期" value-format="YYYY-MM-DD HH:mm:ss"></el-date-picker>
						</div>
						<div class="right-panel">

						</div>
					</el-header>
					<el-header style="height:150px;">
						<scEcharts height="100%" :option="logsChartOption"></scEcharts>
					</el-header>
					<el-main class="nopadding">
						<scTable ref="table" :data="dataList" :page-size="pageSize" @pagination-change="handlePaginationChange" @pagesize="pageSizeChange" stripe highlightCurrentRow @row-click="rowClick">
							<el-table-column label="级别" prop="level" width="50">
								<template #default="scope">
									<el-icon v-if="scope.row.level=='Fatal'" style="color: #F56C6C;"><el-icon-circle-close-filled /></el-icon>
									<el-icon v-if="scope.row.level=='Critical'" style="color: #F56C6C;"><el-icon-circle-close-filled /></el-icon>
									<el-icon v-if="scope.row.level=='Error'" style="color: #F56C6C;"><el-icon-circle-close-filled /></el-icon>
									<el-icon v-if="scope.row.level=='Warning'" style="color: #E6A23C;"><el-icon-warning-filled /></el-icon>
									<el-icon v-if="scope.row.level=='Information'" style="color: #409EFF;"><el-icon-info-filled /></el-icon>
								</template>
							</el-table-column>
							<el-table-column label="ID" prop="ID" width="180"></el-table-column>
							<el-table-column label="日志名" prop="LoggerName" width="150"></el-table-column>
							<el-table-column label="请求接口" prop="RequestPath" width="260"></el-table-column>
							<el-table-column label="请求方法" prop="RequestMethod" width="80"></el-table-column>
							<el-table-column label="用户" prop="UserName" width="50"></el-table-column>
							<el-table-column label="客户端IP" prop="ClientIP" width="100"></el-table-column>
							<el-table-column label="日志时间" prop="TimeStamp" width="200"></el-table-column>
						</scTable>
					</el-main>
				</el-container>
			</el-main>
		</el-container>
	</el-container>

	<el-drawer v-model="infoDrawer" title="日志详情" :size="600" destroy-on-close>
		<info ref="info"></info>
	</el-drawer>
</template>

<script>
	import info from './info'
	import scEcharts from '@/components/scEcharts'

	export default {
		name: 'logger',
		components: {
			info,
			scEcharts
		},
		data() {
			return {
				currentPage: 1, // 当前页数
                pageSize: 10, // 每页显示的条数
				infoDrawer: false,
				logsChartOption: {
					color: ['#409eff','#e6a23c','#f56c6c'],
					grid: {
						top: '0px',
						left: '10px',
						right: '10px',
						bottom: '0px'
					},
					tooltip: {
						trigger: 'axis'
					},
					xAxis: {
						type: 'category',
						boundaryGap: false,
						data: ['2021-07-01', '2021-07-02', '2021-07-03', '2021-07-04', '2021-07-05', '2021-07-06', '2021-07-07', '2021-07-08', '2021-07-09', '2021-07-10', '2021-07-11', '2021-07-12', '2021-07-13', '2021-07-14', '2021-07-15']
					},
					yAxis: {
						show: false,
						type: 'value'
					},
					series: [{
						data: [120, 200, 150, 80, 70, 110, 130, 120, 200, 150, 80, 70, 110, 130, 70, 110],
						type: 'bar',
						stack: 'log',
						barWidth: '15px'
					},
					{
						data: [15, 26, 7, 12, 13, 9, 21, 15, 26, 7, 12, 13, 9, 21, 12, 3],
						type: 'bar',
						stack: 'log',
						barWidth: '15px'
					},
					{
						data: [0, 0, 0, 120, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
						type: 'bar',
						stack: 'log',
						barWidth: '15px'
					}]
				},
				category: [
					{
						label: '系统日志',
						children: [
							{label: 'Information'},						
							{label: 'Critical'},
							{label: 'Warning'},
							{label: 'Error'},
							{label: 'Fatal'}
						]
					},
					// {
					// 	label: '应用日志',
					// 	children: [
					// 		{label: 'selfHelp'},
					// 		{label: 'WechatApp'}
					// 	]
					// }
				],
				date: [],
				dataList:[],
				search: {
					keyword: "",
					level:"",
					startDateTime: null,
                    endDateTime: null
				}
			}
		},
		async mounted() {
			this.upsearch()
		},
		methods: {
		async	upsearch(){
				let response =  await this.$API.system.log.systemList.post({
					pageNumber:this.currentPage,
					pageSize:this.pageSize,
					orderBy: "Id",
					sortDirection: "Descending",
					level:this.search.level,
					startDateTime :this.search.startDateTime,
					endDateTime :this.search.endDateTime
					//keyword:this.search.keyword
				})
				const list = [];
				for (const item of response.data.items) 
				{
					let data = JSON.parse(item.properties);
					data.level = item.level
					data.exception = item.exception
					list.push(data);
				}
				this.$refs.table.total = response.data.totalItems;
				this.dataList=list
	
				response =  await this.$API.system.log.countDailys.get()
				if (response.succeeded) {
					this.logsChartOption.xAxis.data=response.data.xAxis;

					// 获取今天的日期
                   var today = new Date().toISOString().slice(0, 10);
				   // 遍历日期数组
                   for (var i = 0; i < this.logsChartOption.xAxis.data.length; i++) {
                        if (this.logsChartOption.xAxis.data[i] === today) {
                            this.logsChartOption.xAxis.data[i] = "今天";
                        }
                    }

					this.logsChartOption.series[0].data=response.data.informationConut;
					this.logsChartOption.series[1].data=response.data.warningConut;
					this.logsChartOption.series[2].data=response.data.errorConut;
				}
		},
		rowClick(row){
			this.infoDrawer = true
			this.$nextTick(() => {
				this.$refs.info.setData(row)
			})
		},
		//点击分页
		handlePaginationChange(val){		
				this.currentPage = val;
				this.upsearch();
		},
		    pageSizeChange(size){
				this.pageSize = size;
				this.upsearch();
		    },//树点击事件
			categoryClick(data){		
				this.search.level = data.label
				this.upsearch()
			},
			handlePick(val) {
                this.search.startDateTime =val[0]+".000"
				this.search.endDateTime =val[1]+".000"
				this.upsearch()
            }
		}
	}
</script>

<style>
</style>
