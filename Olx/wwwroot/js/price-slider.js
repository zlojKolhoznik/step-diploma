// source: https://www.geeksforgeeks.org/price-range-slider-with-min-max-input-using-html-css-and-javascript/
document.addEventListener('DOMContentLoaded', function () {
    const rangeValue = document.querySelector(".slider-container .price-slider");
    const rangeInputValue = document.querySelectorAll(".range-input input");
    const priceInputValue = 
        document.querySelectorAll(".price-input input");
    let priceGap = (priceInputValue[1].max - priceInputValue[0].min) * .01;
    let minVal = parseInt(rangeInputValue[0].value);
    let maxVal = parseInt(rangeInputValue[1].value);

    rangeValue.style.left = `${(minVal - rangeInputValue[0].min) / (rangeInputValue[0].max - rangeInputValue[0].min) * 100}%`;
    rangeValue.style.right = `${100 - ((maxVal - rangeInputValue[0].min) / (rangeInputValue[1].max - rangeInputValue[1].min)) * 100}%`;

    for (let i = 0; i < priceInputValue.length; i++) {
        priceInputValue[i].addEventListener("input", e => {
    
            // Parse min and max values of the range input 
            let minp = parseInt(priceInputValue[0].value);
            let maxp = parseInt(priceInputValue[1].value);
            let diff = maxp - minp
    
            if (minp < 0) {
                alert("minimum price cannot be less than 0");
                priceInputValue[0].value = 0;
                minp = 0;
            }
    
            // Validate the input values 
            if (maxp > priceInputValue[1].max) {
                alert("maximum price cannot be greater than 10000");
                priceInputValue[1].value = priceInputValue[1].max;
                maxp = priceInputValue[1].max;
            }
    
            if (minp > maxp - priceGap) {
                priceInputValue[0].value = maxp - priceGap;
                minp = maxp - priceGap;
    
                if (minp < priceInputValue[0].min) {
                    priceInputValue[0].value = priceInputValue[0].min;
                    minp = priceInputValue[0].min;
                }
            }
    
            // Check if the price gap is met 
            // and max price is within the range 
            if (diff >= priceGap && maxp <= rangeInputValue[1].max) {
                if (e.target.className === "min-input") {
                    rangeInputValue[0].value = minp;
                    rangeValue.style.left =
                        `${(minp - rangeInputValue[0].min) / (rangeInputValue[0].max - rangeInputValue[0].min) * 100}%`;
                } else {
                    rangeInputValue[1].value = maxp;
                    rangeValue.style.right =
                        `${100 - ((maxp - rangeInputValue[0].min) / (rangeInputValue[1].max - rangeInputValue[1].min)) * 100}%`;
                }
            }
        });
    
        // Add event listeners to range input elements 
        for (let i = 0; i < rangeInputValue.length; i++) {
            rangeInputValue[i].addEventListener("input", e => {
                let minVal =
                    parseInt(rangeInputValue[0].value);
                let maxVal =
                    parseInt(rangeInputValue[1].value);
    
                let diff = maxVal - minVal
    
                // Check if the price gap is exceeded 
                if (diff < priceGap) {
    
                    // Check if the input is the min range input 
                    if (e.target.className === "min-range") {
                        rangeInputValue[0].value = maxVal - priceGap;
                    } else {
                        rangeInputValue[1].value = minVal + priceGap;
                    }
                } else {
    
                    // Update price inputs and range progress 
                    priceInputValue[0].value = minVal;
                    priceInputValue[1].value = maxVal;
                    console.log(maxVal / (rangeInputValue[1].max - rangeInputValue[1].min));
                    rangeValue.style.left =
                        `${(minVal - rangeInputValue[0].min) / (rangeInputValue[0].max - rangeInputValue[0].min) * 100}%`;
                    rangeValue.style.right =
                        `${100 - ((maxVal - rangeInputValue[0].min) / (rangeInputValue[1].max - rangeInputValue[1].min)) * 100}%`;
                }
            });
        }
    }
});