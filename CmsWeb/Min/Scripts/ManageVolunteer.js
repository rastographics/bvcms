$(function(){$.tigerstripe=function(){$("table.grid > tbody > tr").removeClass("alt"),$("table.grid > tbody > tr:even").addClass("alt")},$.tigerstripe(),$("#sortweek").click(function(n){n.preventDefault(),$("table.grid > tbody > tr").sortElements(function(n,t){return $(n).find("td.week").text()>$(t).find("td.week").text()?1:-1}),$.tigerstripe()}),$("#sortday").click(function(n){n.preventDefault(),$("table.grid > tbody > tr").sortElements(function(n,t){return $(n).find("td.day").attr("jday")>$(t).find("td.day").attr("jday")?1:-1}),$.tigerstripe()})})