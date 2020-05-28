namespace QSThumbCreator.Processor
{
    public class QlikThumbProcessorJs
    {
        /*
         * We have to wait for a few things to happen before we can take a snapshot.
         * The below JS is injected into the browser and is used to make sure the necessary
         * events have happened.
         *
         * 1. Make sure that the pong waiting screen is no longer visible
         * 2. Wait at least 1 AFTER the receiving of the last paint/navigation/resource call is
         *    made in the browser.
         */
        public static string WaitForSheetLoadJsChrome = @"
            let performance = window.performance;
            window.startPerfTime = performance.timeOrigin;
            window.lastEndingTime = performance.timeOrigin;
            console.log(""Performance Time Origin: "" + performance.timeOrigin);
            performance.mark(""Beginning perf monitoring"");
            
            window.oneSecondPastLastEvent = function()
            {
                let currentTime = Date.now();
                let timeValue = window.startPerfTime + window.lastEndingTime;
                let canProceed = false;
                if( document.querySelector('div.qs-pong') !== undefined && document.querySelector('div.qs-pong') !== null)
                {
                    window.lastEndingTime = Date.now();
                    console.log(""Pong div is still active at:"" + currentTime + "" window.lastEndingTime = "" + window.lastEndingTime);
                    canProceed = false;
                }
                else
                {
                    canProceed =  window.lastEndingTime + 2000 < currentTime;
                }

                var statsObject = {};
                statsObject.type = 'WINDOW_FUNC_CALL';
                statsObject.startPerfTime = window.startPerfTime;
                statsObject.lastEndingTime = window.lastEndingTime;
                statsObject.currentTime = currentTime;
                statsObject.timeValueDiff =  currentTime - window.lastEndingTime;

                console.log(statsObject);

                return canProceed;
            }

            function perf_observer(list, observer) {
                list.getEntries().forEach( (performanceEntry, i, entries) => {
                    let currentTime = Date.now();
                    let endingTime = performanceEntry.startTime + performanceEntry.duration;
                    if( window.lastEndingTime < endingTime )   
                    {
                        window.lastEndingTime =  endingTime
                    }
                
                    var statsObject = {};
                    statsObject.type = 'PERF_OBSERVER';
                    statsObject.entryStartPerfTime = performanceEntry.startTime;
                    statsObject.entryDuration = performanceEntry.duration;
                    statsObject.entryEndingTime = endingTime;
                    statsObject.windowStartPerfTime = window.startPerfTime;
                    statsObject.windowLastEndingTime = window.lastEndingTime;
                    statsObject.currentTime = currentTime;

                    console.log(statsObject);
                });
            }
    
            var observer2 = new PerformanceObserver(perf_observer);
            observer2.observe({entryTypes: [""paint"",""resource"",""navigation""]});";
    }
}