import { Component, OnInit } from '@angular/core';
import { EChartOption } from 'echarts';
import { echartStyles } from '../../../shared/echart-styles';
import { DashboardEcommerce, DashboardVentas } from '../../../shared/models/dashboards.model';
import { DashboardsService } from '../../../shared/services/dashboards.service';
import { TipoDashboards } from '../../../shared/enums/TipoDashboards';

@Component({
	selector: 'app-dashboad-default',
	templateUrl: './dashboad-default.component.html',
	styleUrls: ['./dashboad-default.component.css']
})
export class DashboadDefaultComponent implements OnInit {
	chartLineOption1: EChartOption;
	chartLineOption2: EChartOption;
	chartLineOption3: EChartOption;
    salesChartBar: EChartOption;

    dashboardEcommerceResult: DashboardEcommerce = new DashboardEcommerce();
    dashboardVentasResult: DashboardVentas = new DashboardVentas();

	constructor(private dashboardsService: DashboardsService) { }

	ngOnInit() {
		this.chartLineOption1 = {
			...echartStyles.lineFullWidth, ...{
				series: [{
					data: [30, 40, 20, 50, 40, 80, 90],
					...echartStyles.smoothLine,
					markArea: {
						label: {
							show: true
						}
					},
					areaStyle: {
						color: 'rgba(102, 51, 153, .2)',
						origin: 'start'
					},
					lineStyle: {
						color: '#663399',
					},
					itemStyle: {
						color: '#663399'
					}
				}]
			}
		};
		this.chartLineOption2 = {
			...echartStyles.lineFullWidth, ...{
				series: [{
					data: [30, 10, 40, 10, 40, 20, 90],
					...echartStyles.smoothLine,
					markArea: {
						label: {
							show: true
						}
					},
					areaStyle: {
						color: 'rgba(255, 193, 7, 0.2)',
						origin: 'start'
					},
					lineStyle: {
						color: '#FFC107'
					},
					itemStyle: {
						color: '#FFC107'
					}
				}]
			}
		};
		this.chartLineOption2.xAxis.data = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];

		this.chartLineOption3 = {
			...echartStyles.lineNoAxis, ...{
				series: [{
					data: [40, 80, 20, 90, 30, 80, 40, 90, 20, 80, 30, 45, 50, 110, 90, 145, 120, 135, 120, 140],
					lineStyle: {
						color: 'rgba(102, 51, 153, 0.86)',
						width: 3,
						...echartStyles.lineShadow
					},
					label: { show: true, color: '#212121' },
					type: 'line',
					smooth: true,
					itemStyle: {
						borderColor: 'rgba(102, 51, 153, 1)'
					}
				}]
			}
		};
		// this.chartLineOption3.xAxis.data = ['1', '2', '3', 'Thu', 'Fri', 'Sat', 'Sun'];
        this.salesChartBar = {
            legend: {
                borderRadius: 0,
                orient: 'horizontal',
                x: 'right',
                data: ['Ecommerce', 'Softland']
            },
            grid: {
                left: '8px',
                right: '8px',
                bottom: '0',
                containLabel: true
            },
            tooltip: {
                show: true,
                backgroundColor: 'rgba(0, 0, 0, .8)'
            },
            xAxis: [{
                type: 'category',
                data: ['Lunes', 'Martes', 'Miercoles', 'Viernes', 'Sabado', 'Domingo'],
                axisTick: {
                    alignWithLabel: true
                },
                splitLine: {
                    show: false
                },
                axisLine: {
                    show: true
                }
            }],
            yAxis: [{
                    type: 'value',
                    axisLabel: {
                        formatter: '${value}'
                    },
                    min: 0,
                    max: 100000,
                    interval: 25000,
                    axisLine: {
                        show: false
                    },
                    splitLine: {
                        show: true,
                        interval: 'auto'
                    }
                }

            ],

            series: [{
                    name: 'Ecommerce',
                    data: [35000, 69000, 22500, 60000, 50000, 50000, 30000],
                    label: { show: false, color: '#0168c1' },
                    type: 'bar',
                    barGap: 0,
                    color: '#bcbbdd',
                    smooth: true,

                },
                {
                    name: 'Softland',
                    data: [45000, 82000, 35000, 93000, 71000, 89000, 49000],
                    label: { show: false, color: '#639' },
                    type: 'bar',
                    color: '#7569b3',
                    smooth: true
                }

            ]
        };

        this.getDashboardEcommerce();
    }
    
    getDashboardEcommerce() {
        const data: DashboardEcommerce = {
            tipoDashboard: TipoDashboards.Ecommerce,
            fechaDesde: new Date(2019, new Date().getMonth(), new Date().getDate(), 0, 0, 0),
            fechaHasta: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 23, 59, 59)
        };

        this.dashboardsService.getDashboardEcommerce(data).subscribe((res: DashboardEcommerce) => {
            this.dashboardEcommerceResult = res;            
        }, err => { });
    }

}
