function starRating(rating, elementId) {
    for (var i = 0; i < 5; i++) {
        var ionIcon = document.createElement("ion-icon");
        if (i < rating) {
            ionIcon.setAttribute("name", "star");
        } else {
            ionIcon.setAttribute("name", "star-outline");
        }
        document.getElementById(elementId).appendChild(ionIcon);
    }
}