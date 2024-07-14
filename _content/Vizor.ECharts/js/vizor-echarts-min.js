window.vizorECharts={charts:new Map,dataSources:new Map,logging:!1,changeLogging:function(t){vizorECharts.logging=t},getChart:function(t){return vizorECharts.charts.get(t)},getDataSource:function(t){var a=vizorECharts.dataSources.get(t);return vizorECharts.logging&&(console.log(`GET CACHED FETCH ${t}`),console.log(a)),a},evaluatePath:function(t,a){const e=a.split(".");let o=t;for(const t of e){if(!o.hasOwnProperty(t))return;o=o[t]}return o},fetchExternalData:async function(chart,fetchOptions){if(null!=fetchOptions)for(item of(chart.__dataSources=[],JSON.parse(fetchOptions))){vizorECharts.logging&&(console.log(`FETCH ${item.id}`),console.log(item));const response=await fetch(item.url,item.options);if(!response.ok)throw new Error("Failed to fetch external chart data: url="+url);var data=null;if("json"==item.fetchAs){if(data=await response.json(),null!=item.path)try{data=vizorECharts.evaluatePath(data,item.path)}catch(t){console.log("Failed to evaluate path expression of external data source"),console.log(t)}}else"string"==item.fetchAs&&(data=await response.text());if(vizorECharts.logging&&console.log(data),null!=item.afterLoad)try{const func=eval(`(${item.afterLoad})`);data=func(data)}catch(t){console.log("Failed to execute afterLoad function of external data source"),console.log(t)}window.vizorECharts.dataSources.set(item.id,data),chart.__dataSources.push(item.id)}},registerMaps:function(chart,mapOptions){if(null!=mapOptions){var parsedOptions=eval("("+mapOptions+")");for(item of parsedOptions)vizorECharts.logging&&(console.log("MAP"),console.log(item)),"geoJSON"===item.type?echarts.registerMap(item.mapName,{geoJSON:item.geoJSON,specialAreas:item.specialAreas}):"svg"===item.type&&echarts.registerMap(item.mapName,{svg:item.svg})}},initChart:async function(id,theme,initOptions,chartOptions,mapOptions,fetchOptions){var chart=echarts.init(document.getElementById(id),theme,JSON.parse(initOptions));if(vizorECharts.charts.set(id,chart),chart.showLoading(),null!=chartOptions){await vizorECharts.fetchExternalData(chart,fetchOptions),await vizorECharts.registerMaps(chart,mapOptions);var parsedOptions=eval("("+chartOptions+")");vizorECharts.logging&&(console.log("CHART"),console.log(parsedOptions)),chart.setOption(parsedOptions),chart.hideLoading()}},updateChart:async function(id,chartOptions,mapOptions,fetchOptions){var chart=vizorECharts.charts.get(id);if(null!=chart){await vizorECharts.fetchExternalData(chart,fetchOptions),await vizorECharts.registerMaps(chart,mapOptions);var parsedOptions=eval("("+chartOptions+")");chart.setOption(parsedOptions),chart.hideLoading()}else console.error("Failed to retrieve chart "+id)},disposeChart:function(t){var a=vizorECharts.charts.get(t);null!=a?(a.__dataSources&&Array.isArray(a.__dataSources)&&a.__dataSources.forEach(t=>{window.vizorECharts.dataSources.delete(t)}),echarts.dispose(a),vizorECharts.charts.delete(t)):console.error("Failed to dispose chart "+t)}};