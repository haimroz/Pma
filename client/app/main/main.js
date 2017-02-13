/**
 * Created by nir.moreno on 21/12/2016.
 */
'use strict';

var _timeout = 500; //threshold for auto-play

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
        $scope.Timer = null;
        $scope.options = {
            from: 0,
            to: 59,
            step: 1,
            dimension: " sec",
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

        $scope.showGotoKB = function() {
            if ($scope.data)
            {
                if (!$scope.data[$scope.value][13]) return true;
            }
            return false;
            // return $scope.invalidTimeSlot;
        };

        $scope.goToKB = function () {
            $window.open('https://sites.google.com/a/zerto.com/dev/home/zzz-deprecated-pages-1/vrastoragebottleneckkb');
        };
        $scope.toggleButton = function () {
            $scope.isPlayed = !$scope.isPlayed;
            $scope.isPlayed ? StartTimer() : StopTimer();
        };


        //Private
        function onGetDataSuccess(data) {
            $scope.data = processData(data) || [];
            $scope.thresholds = processThresholds(data) || [];
            $scope.options.to = data.length - 1;
            $scope.options.from = 0;
            $scope.value = $scope.invalidTimeSlot ? $scope.invalidTimeSlot_index : 0;

            //Remove the String in the head
            renderChart($scope.data[$scope.value], $scope.thresholds[$scope.value]);
        }

        function processThresholds(data) {
            var result = [];
            data.forEach(function (timeSlot) {
                var processedTimeSlot = [];

                timeSlot.PmaRawFieldList.forEach(function (item) {
                    processedTimeSlot.push(item.Threshold);
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

            renderChart($scope.data[value], $scope.thresholds[value]);
        }

        function renderChart(elements, thresholds) {

            $scope.chart = c3.generate({
                data: {
                    x: 'x',
                    columns: [
                        ['x', 'ProtectedVolumeWriteRateMbs', 'ProtectedVolumeCompressedWriteRateMBs', 'ProtectedCpuPerc', 'ProtectedVraBufferUsagePerc', 'ProtectedTcpBufferUsagePerc', 'NetworkOutgoingRateMBs', 'RecoveryTcpBufferUsagePerc', 'RecoveryCpuPerc', 'RecoveryVraBufferUsagePerc', 'HardeningRateMBs', 'JournalSizeMB', 'ApplyRateMBs'],
                        elements
                    ],
                    type: 'bar'
                },
                legend: {
                    show: false
                },
                axis: {
                    x: {
                        type: 'category',
                        tick: {
                            rotate: 0, //axis labels rotation angle
                            multiline: true
                        },
                        height: 130,

                    },
                    y: {
                        padding: {
                            top: 50
                        },
                        tick: {
                            format: d3.format("0.%"),
                            values: [0, 50, 100]
                        }
                    }
                }
            });


            // where to draw the target lines for each data point
            var scalingFactors = thresholds;

            // svg layer for each bar series
            var barsLayers = $scope.chart.internal.main.selectAll('.' + c3.chart.internal.fn.CLASS.bars)[0];
            var bars = $scope.chart.internal.main.selectAll('.' + c3.chart.internal.fn.CLASS.bar)[0];
            // use the same function c3 uses to get each bars corners
            var getPoints = $scope.chart.internal.generateGetBarPoints($scope.chart.internal.getShapeIndices($scope.chart.internal.isBarType));
            // just in case there are multiple series
            $scope.chart.internal.data.targets.forEach(function (series, i) {
                // for each point in the series
                series.values.forEach(function (d, j) {
                    // highlight if over threshold
                    if (d.value > scalingFactors[j])
                        d3.select(bars[j]).classed('crossed', true);

                    else if (d.value === scalingFactors[j])
                        d3.select(bars[j]).classed('equal', true);

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
                    return;
                } else {
                    $scope.value++;
                    renderChart($scope.data[$scope.value], $scope.thresholds[$scope.value]);
                }
            }, _timeout);
        }

        function StopTimer() {

            //Set the Timer stop message.
            $scope.Message = "Timer stopped.";

            //Cancel the Timer.
            if (angular.isDefined($scope.Timer)) {
                $interval.cancel($scope.Timer);
            }
        }

        serviceFactory.getData().then(onGetDataSuccess);
    }]);