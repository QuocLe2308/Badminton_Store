$(document).ready(function () {
    function formatVND(amount) {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
    }
    $(".add-to-cart").click(function (e) {
        e.preventDefault();

        var productId = $(this).data("id");
        var productName = $(this).data("name");
        var productPrice = $(this).data("price");
        var productQuantity = 1; // Số lượng mặc định là 1 khi thêm sản phẩm vào giỏ hàng
        var productImage = $(this).data("image"); // Lấy đường dẫn hình ảnh từ thuộc tính data-image

        // Get current cart from cookies
        var cart = JSON.parse(getCookie("cart") || "[]");

        // Check if the product already exists in the cart
        var existingProduct = cart.find(p => p.id === productId);
        if (existingProduct) {
            existingProduct.quantity += productQuantity; // Increase quantity if product exists
        } else {
            // Add new product to cart
            cart.push({
                id: productId,
                name: productName,
                price: productPrice,
                quantity: productQuantity,
                image: productImage // Thêm đường dẫn hình ảnh vào đối tượng sản phẩm trong giỏ hàng
            });
        }

        // Save updated cart to cookies
        setCookie("cart", JSON.stringify(cart), 30);

        alert("Product added to cart!");
        updateShoppingCartFromCookie();
    });

    function setCookie(name, value, days) {
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + (value || "") + expires + "; path=/";
    }

    function updateShoppingCartFromCookie() {
        var cart = JSON.parse(getCookie("cart") || "[]");
        var cartContainer = $(".shopping-cart");

        // Clear existing cart items
        cartContainer.find(".shipping-item").remove();

        // Loop through cart items in cookie
        cart.forEach(function (item) {
            var productHtml = `
                <div class="shipping-item">
                    <span class="cross-icon"><i class="fa fa-times-circle"></i></span>
                    <div class="shipping-item-image">
                        <a href="#"><img src="${item.image}" alt="shopping image" /></a>
                    </div>
                    <div class="shipping-item-text">
                        <span>${item.quantity} <span class="pro-quan-x">x</span> <a href="#" class="pro-cat">${item.name}</a></span>
                        <span class="pro-quality"><a href="#">Size, Color</a></span>
                        <p>${formatVND(item.price)}</p>
                    </div>
                </div>`;

            cartContainer.find(".shipping-total-bill").before(productHtml);
        });

        // Update total quantity
        var totalQuantity = cart.reduce((total, item) => total + item.quantity, 0);
        cartContainer.find(".ajax-cart-quantity").text(totalQuantity);

        // Calculate and update total price
        var totalPrice = cart.reduce((total, item) => total + (item.price * item.quantity), 0);
        var shippingCost = 2; // Example shipping cost
        var totalBill = totalPrice + shippingCost;

        cartContainer.find(".shipping-cost").text(formatVND(shippingCost));
        cartContainer.find(".shipping-total").text(formatVND(totalBill));
    }

    // Initial update when the page loads
    updateShoppingCartFromCookie();

    // Function to delete an item from cart (not implemented here, just as an example)
    $(".shipping-cart-overly").on("click", ".cross-icon", function () {
        // Implement deletion logic here
        // Then update cart display
        updateShoppingCartFromCookie();
    });

    // Function to retrieve cookie value by name
    function getCookie(name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    }
    const shippingCost = 0.00;

    function getCookie(name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    }

    function loadCart() {
        var cart = JSON.parse(getCookie("cart") || "[]");
        var cartBody = $("#cart-body");
        cartBody.empty();

        var totalProductCost = 0;

        cart.forEach(function (product) {
            var productTotal = product.price * product.quantity;
            totalProductCost += productTotal;

            var cartItem = `
                <tr>
                    <td class="cart-product">
                        <a href="#"><img alt="${product.name}" src="${product.image}" width="50"></a>
                    </td>
                    <td class="cart-description">
                        <p class="product-name"><a href="#">${product.name}</a></p>
                        <small>SKU : ${product.id}</small>
                        <small><a href="#">Size : S, Color : Orange</a></small>
                    </td>
                    <td class="cart-avail"><span class="label label-success">In stock</span></td>
                    <td class="cart-unit">
                        <ul class="price text-right">
                            <li class="price">${formatVND(product.price)}</li>
                        </ul>
                    </td>
                    <td class="cart_quantity text-center">
                        <div class="cart-plus-minus-button">
                            
                            <input class="cart-plus-minus" type="text" name="qtybutton" value="${product.quantity}" readonly>
                            <div class="dec qtybutton" data-id="${product.id}">-</div><div class="inc qtybutton" data-id="${product.id}">+</div>
                        </div>
                    </td>
                    <td class="cart-delete text-center">
                        <span>
                            <a href="#" class="cart_quantity_delete" data-id="${product.id}" title="Delete"><i class="fa fa-trash-o"></i></a>
                        </span>
                    </td>
                    <td class="cart-total">
                        <span class="price">${formatVND(productTotal)}</span>
                    </td>
                </tr>
            `;
            cartBody.append(cartItem);
        });

        updateTotals(totalProductCost);

        // Add delete functionality
        $(".cart_quantity_delete").click(function (e) {
            e.preventDefault();
            var productId = $(this).data("id");
            removeFromCart(productId);
        });

        // Add plus/minus functionality
        $(".inc.qtybutton").click(function () {
            var productId = $(this).data("id");
            updateQuantity(productId, 1);
        });

        $(".dec.qtybutton").click(function () {
            var productId = $(this).data("id");
            updateQuantity(productId, -1);
        });
    }

    function updateQuantity(productId, change) {
        var cart = JSON.parse(getCookie("cart") || "[]");
        var product = cart.find(p => p.id === productId);
        if (product) {
            product.quantity += change;
            if (product.quantity <= 0) {
                cart = cart.filter(p => p.id !== productId);
            }
            setCookie("cart", JSON.stringify(cart), 30);
            loadCart();
        }
    }

    function removeFromCart(productId) {
        var cart = JSON.parse(getCookie("cart") || "[]");
        cart = cart.filter(product => product.id !== productId);
        setCookie("cart", JSON.stringify(cart), 30);
        loadCart();
    }

    function setCookie(name, value, days) {
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + (value || "") + expires + "; path=/";
    }

    function updateTotals(totalProductCost) {
        $("#total_product").text(`${formatVND(totalProductCost)}`);
        var totalCost = totalProductCost + shippingCost;
        $("#total-price").text(`${formatVND(totalCost)}`);
    }

    // Initial load
    loadCart();


    // Lắng nghe sự kiện khi nhập vào các input
    $('#inputAddress, #inputWard, #inputDistrict, #inputProvince, #inputFullName, #inputPhoneNumber, #inputEmail').on('keyup', function () {
        console.log("!23123");
        // Lấy giá trị từ các input
        var address = $('#inputAddress').val();
        var ward = $('#inputWard').val();
        var district = $('#inputDistrict').val();
        var province = $('#inputProvince').val();
        var fullName = $('#inputFullName').val();
        var phoneNumber = $('#inputPhoneNumber').val();
        var email = $('#inputEmail').val();

        // Cập nhật các phần tử trong ul.address
        $('.address_name').text(fullName);
        $('.address_address_full').text(address + ", " + ward + ", " + district + ", " + province);
        $('.address_phone').text(phoneNumber);
        $('.address_email').text(email);
        // Lưu thông tin vào cookie
        var infoShipping = {
            fullName: fullName,
            address: address,
            ward: ward,
            district: district,
            province: province,
            phoneNumber: phoneNumber,
            email: email
        };

        // Chuyển đổi thành JSON và lưu vào cookie trong 30 ngày
        setCookie('infoshipping', JSON.stringify(infoShipping), 30);


    });
    function loadShippingInfoFromCookie() {
        var cookieValue = getCookie('infoshipping');
        if (cookieValue) {
            var infoShipping = JSON.parse(cookieValue);

            // Populate input fields with cookie data
            $('#inputAddress').val(infoShipping.address);
            $('#inputWard').val(infoShipping.ward);
            $('#inputDistrict').val(infoShipping.district);
            $('#inputProvince').val(infoShipping.province);
            $('#inputFullName').val(infoShipping.fullName);
            $('#inputPhoneNumber').val(infoShipping.phoneNumber);
            $('#inputEmail').val(infoShipping.email);

            // Update displayed address details if necessary
            $('.address_name').text(infoShipping.fullName);
            $('.address_address_full').text(infoShipping.address + ", " + infoShipping.ward + ", " + infoShipping.district + ", " + infoShipping.province);
            $('.address_phone').text(infoShipping.phoneNumber);
            $('.address_email').text(infoShipping.email);
        }
    }

    // Call the function to load shipping info from cookie on page load
    loadShippingInfoFromCookie();



    
});