let dataElements = document.getElementsByClassName('js-date-format');
    const NOW = new Date();
    for(let index in dataElements) {
        let date = new Date(dataElements[index].innerText);
        const diffTime = Math.abs(NOW - date);
        const diffSeconds = Math.floor(diffTime / (1000)); 
        const diffMinutes = Math.floor(diffSeconds / (60)); 
        const diffHours = Math.floor(diffMinutes / (60)); 
        console.log(diffTime);
        console.log(diffSeconds);
        console.log(diffMinutes);
        console.log(diffHours);
        let printDate = date.toISOString();
        if (diffHours == 0) {
            if (diffMinutes > 0) {
                printDate = `${diffMinutes} minute${diffMinutes > 1 ? 's' : ''} ago`;    
            } else {
                printDate = `${diffSeconds} second${diffSeconds > 1 ? 's' : ''} ago`;    
            }
        }
        if (diffHours > 0 && diffHours < 24) {
            printDate = `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`;
        } 
        dataElements[index].innerText = printDate;
    }