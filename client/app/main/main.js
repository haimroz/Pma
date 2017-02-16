/**
 * Created by nir.moreno on 21/12/2016.
 */
'use strict';

const _timeFrame = 100;
const _viewName = "viewName";
const _pageSize = 100;

angular.module('myApp.main', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        $routeProvider.when('/main', {
            templateUrl: 'main/main.html',
            controller: 'MainCtrl'
        });
    }])
    .controller('MainCtrl', ['$scope', 'serviceFactory', '$interval', '$window', function ($scope, serviceFactory, $interval, $window) {

        //Slider
        $scope.isPlayed = false;
        $scope.value = 0;
        $scope.showLoader = false;
        $scope.currentIndexPage = 1;
        $scope.maxIndexPage = 0;
        $scope.protectedVraFilePath = undefined;
        $scope.recoveryVraFilePath = undefined;
        $scope.sliderSensitivity = 5;
        $scope.Timer = null;
        $scope.options = {
            from: 0,
            to: 59,
            step: 1,
            dimension: " slot",
            smooth: true,
            realtime: true,
            css: {
                background: {"background-color": "silver"},
                before: {"background-color": "green"},
                default: {"background-color": "white"},
                after: {"background-color": "green"},
                pointer: {"background-color": "#d1c9b8"}
            },
            fontSize: 40,
            callback: onDragSlider
        };
        $scope.Message = 'Timer paused';


        $scope.showGotoKB = function () {
            // if ($scope.data) {
            //     if (!$scope.data[$scope.value][13]) return true;
            // }
            // return false;
            return $scope.invalidTimeSlot;
        };
        $scope.goToKB = function () {
            $window.open('https://sites.google.com/a/zerto.com/dev/home/zzz-deprecated-pages-1/vrastoragebottleneckkb');
        };
        $scope.toggleButton = function () {
            $scope.isPlayed = !$scope.isPlayed;
            $scope.isPlayed ? StartTimer() : StopTimer();
        };
        $scope.onSliderSensitivityChanged = function () {
            if ($scope.Timer) {
                StopTimer();
                StartTimer();
            }
        };
        $scope.onClickParseLogs = function () {
            getDataFromServer($scope.protectedVraFilePath, $scope.recoveryVraFilePath, $scope.currentIndexPage);
        };
        $scope.onNewIndexClicked = function (isNext) {
            if (isNext) {
                if ($scope.currentIndexPage < $scope.maxIndexPage) {
                    $scope.currentIndexPage++;
                } else {
                    return;
                }

            } else {
                if ($scope.currentIndexPage > 1) {
                    $scope.currentIndexPage--;
                } else {
                    return;
                }
            }
            getDataFromServer($scope.protectedVraFilePath, $scope.recoveryVraFilePath, $scope.currentIndexPage).then(StopTimer);
        };

        //Private
        function onGetDataSuccess(data) {
            $scope.showLoader = false;
            if (!data)return;
            $scope.maxIndexPage = Math.ceil(data.Count / _pageSize);
            $scope.data = processData(data.PmaData) || [];
            $scope.thresholds = processThresholds(data.PmaData) || [];
            $scope.options.to = data.PmaData.length - 1;
            $scope.options.from = 0;
            $scope.value = $scope.invalidTimeSlot ? $scope.invalidTimeSlot_index : 0;

            //Remove the String in the head
            renderChart($scope.data[$scope.value], $scope.thresholds[$scope.value]);
        }

        function getDataFromServer(protectedVraFilePath, recoveryVraFilePath, currentIndexPage) {
            $scope.showLoader = true;
            return serviceFactory.getData(protectedVraFilePath, recoveryVraFilePath, _pageSize, currentIndexPage)
                .then(onGetDataSuccess);
        }

        function processThresholds(data) {
            var result = [];
            data.forEach(function (timeSlot) {
                var processedTimeSlot = [];

                timeSlot.PmaRawFieldList.forEach(function (item) {
                    processedTimeSlot.push(parseInt(item.Threshold, 10));
                });

                result.push(processedTimeSlot);

            });
            return result;
        }

        function processData(data) {
            var result = [];
            data.forEach(function (timeSlot, index) {
                var processedTimeSlot = [];
                processedTimeSlot.push(timeSlot["TimeStamp"]);
                timeSlot.PmaRawFieldList.forEach(function (item) {
                    processedTimeSlot.push(item.Value);
                });

                processedTimeSlot.push(timeSlot.IsValid);

                if (!timeSlot.IsValid && !$scope.invalidTimeSlot) {
                    $scope.invalidTimeSlot = processedTimeSlot;
                    $scope.invalidTimeSlot_index = index;
                }
                result.push(processedTimeSlot);
            });
            return result;
        }

        function onDragSlider(value, released) {
            // useful when combined with 'realtime' option
            // released it triggered when mouse up
            console.log(value + " " + released);

            loadChart($scope.data[value], $scope.thresholds[value]);
        }

        function loadChart(elements, thresholds) {
            $scope.chart.load({
                columns: [
                    [_viewName].concat(elements)
                ]
            });
            drawChartBarLayers(thresholds);
        }

        function renderChart(elements, thresholds) {
            $scope.chart = c3.generate({
                size: {
                    height: 400
                },
                interaction: {
                    enabled: true
                },
                transition: {
                    duration: _timeFrame * $scope.sliderSensitivity
                },
                data: {
                    x: 'xLabels',
                    columns: [
                        ['xLabels',
                            'Protected Volume Write Rate Mbs',
                            'Protected Volume Compressed Write Rate MBs',
                            'Protected Cpu Perc',
                            'Protected Vra Buffer Usage Perc',
                            'Protected Tcp Buffer Usage Perc',
                            'Network Outgoing Rate MBs',
                            'Recovery Tcp Buffer Usage Perc',
                            'Recovery Cpu Perc',
                            'Recovery Vra Buffer Usage Perc',
                            'Hardening Rate MBs',
                            'Journal Size MB',
                            'ApplyRateMBs'],
                        [_viewName].concat(elements)
                    ],
                    type: 'bar',
                    selection: {
                        draggable: true
                    }
                },
                bar: {
                    width: {ratio: 0.6}
                },
                legend: {
                    show: true
                },
                axis: {
                    x: {
                        type: 'category',
                        tick: {
                            width: 100,
                            rotate: 0, //axis labels rotation angle
                            multiline: true
                        },
                        height: 70
                    },
                    y: {
                        padding: {
                            top: 50
                        },
                        tick: {
                            values: [0, 50, 70, 100, 150, 200],
                            centered: true,
                            culling: true,
                            outer: true,
                            max: 150,
                            count: 5
                        },
                        max: 200
                    }
                }
            });
            drawChartBarLayers(thresholds);
        }

        function drawChartBarLayers(thresholds) {
            // where to draw the target lines for each data point
            var scalingFactors = ["0"].concat(thresholds);

            // svg layer for each bar series
            var barsLayers = $scope.chart.internal.main.selectAll('.' + c3.chart.internal.fn.CLASS.bars)[0];
            var bars = $scope.chart.internal.main.selectAll('.' + c3.chart.internal.fn.CLASS.bar)[0];
            // use the same function c3 uses to get each bars corners
            var getPoints = $scope.chart.internal.generateGetBarPoints($scope.chart.internal.getShapeIndices($scope.chart.internal.isBarType));
            // just in case there are multiple series


            $scope.chart.internal.data.targets.forEach(function (series, i) {
                // for each point in the series
                d3.select(barsLayers[i]).selectAll("line").remove();

                series.values.forEach(function (d, j) {
                    // highlight if over threshold
                    if (d.x === 0) {
                        return;
                    }
                    if (d.value > scalingFactors[j])
                        d3.select(bars[j]).classed('crossed', true);

                    else if (d.value === scalingFactors[j])
                        d3.select(bars[j]).classed('equal', true);

                    else {
                        d3.select(bars[j]).classed('crossed', false);
                        d3.select(bars[j]).classed('equal', false);
                    }

                    // get the position for our target lines
                    var value = d.value;
                    d.value = scalingFactors[j];
                    var pos = getPoints(d, j);
                    d.value = value;

                    var posTopLeft = pos[1];
                    var posTopRight = pos[2];

                    // draw target lines
                    d3.select(barsLayers[i]).append("line")
                        .attr("x1", posTopLeft[0] - 10)
                        .attr("y1", posTopLeft[1])
                        .attr("x2", posTopRight[0] + 10)
                        .attr("y2", posTopRight[1])
                        .attr("stroke-width", 2)
                        .style("stroke-dasharray", ("6, 4"));
                });
            });
        }

        function StartTimer() {
            //Set the Timer start message.
            $scope.Message = "Timer started. ";

            //Initialize the Timer to run every 1000 milliseconds i.e. one second.
            $scope.Timer = $interval(function () {
                if ($scope.value >= $scope.options.to) {
                    StopTimer();
                    $scope.isPlayed = !$scope.isPlayed;
                    return;
                } else {
                    $scope.value++;
                    loadChart($scope.data[$scope.value], $scope.thresholds[$scope.value]);
                }
            }, $scope.sliderSensitivity * _timeFrame);
        }

        function StopTimer() {

            //Set the Timer stop message.
            $scope.Message = "Timer stopped.";

            //Cancel the Timer.
            if (angular.isDefined($scope.Timer)) {
                $interval.cancel($scope.Timer);
                $scope.Timer = null;
            }
        }
    }]);