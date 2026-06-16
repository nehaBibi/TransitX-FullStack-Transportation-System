// TransitX – Full-Stack JavaScript
// Handles: booking modal, route search, live tracking, booking tracker

document.addEventListener('DOMContentLoaded', () => {

    // ── Video Controls ──────────────────────────────────────────────────────
    const video = document.getElementById('heroVideo');
    const playPauseBtn = document.getElementById('playPauseBtn');
    const videoTimer = document.getElementById('videoTimer');
    const videoDuration = document.getElementById('videoDuration');

    if (video && playPauseBtn) {
        const fmt = s => `${Math.floor(s / 60)}:${String(Math.floor(s % 60)).padStart(2, '0')}`;
        video.addEventListener('loadedmetadata', () => { videoDuration.textContent = fmt(video.duration); });
        video.addEventListener('timeupdate', () => { videoTimer.textContent = fmt(video.currentTime); });
        playPauseBtn.addEventListener('click', () => {
            if (video.paused) { video.play(); playPauseBtn.innerHTML = '<i class="fa-solid fa-pause"></i>'; }
            else { video.pause(); playPauseBtn.innerHTML = '<i class="fa-solid fa-play"></i>'; }
        });
    }

    // ── Route Search (AJAX) ────────────────────────────────────────────────
    const searchInput = document.getElementById('searchInput');
    const routeContainer = document.getElementById('routeContainer');
    let searchTimeout;

    if (searchInput) {
        searchInput.addEventListener('input', () => {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => fetchRoutes(searchInput.value), 300);
        });
    }

    async function fetchRoutes(q) {
        try {
            const res = await fetch(`/Home/Routes?q=${encodeURIComponent(q)}`);
            const routes = await res.json();
            renderRoutes(routes);
        } catch (e) { console.error(e); }
    }

    function renderRoutes(routes) {
        if (!routeContainer) return;
        if (!routes.length) {
            routeContainer.innerHTML = `<div class="col-span-2 text-center py-12 text-slate-400 text-lg">No routes found matching your search.</div>`;
            return;
        }
        routeContainer.innerHTML = routes.map(r => {
            const isOnTime = r.status === 'On Time';
            const statusBadge = isOnTime
                ? `<span class="inline-block px-3 py-1 rounded-full bg-green-500/20 text-green-300 text-sm font-semibold">✓ On Time</span>`
                : `<span class="inline-block px-3 py-1 rounded-full bg-red-500/20 text-red-300 text-sm font-semibold">⚠ Delayed ${r.delayInfo}</span>`;
            const seatColor = r.availableSeats < 5 ? 'text-red-400' : 'text-green-400';
            const bookBtn = r.availableSeats > 0
                ? `<button class="book-btn w-full mt-4 px-4 py-2 bg-cyan-500 hover:bg-cyan-600 text-white font-semibold rounded-lg smooth-transition glow-hover cursor-pointer"
                        data-route-id="${r.id}" data-route="${r.origin} → ${r.destination}"
                        data-price="${r.price}" data-seats="${r.availableSeats}">
                        Book Now – PKR ${r.price.toLocaleString()}
                   </button>`
                : `<button class="w-full mt-4 px-4 py-2 bg-slate-600 text-slate-400 font-semibold rounded-lg cursor-not-allowed" disabled>Sold Out</button>`;

            return `
            <div class="route-card p-6 rounded-xl bg-gradient-to-br from-slate-800/50 to-slate-900/50 border border-slate-700/50 smooth-transition glow-hover hover:border-cyan-500/30 overflow-hidden">
                <img src="${r.imageUrl}" alt="${r.origin} → ${r.destination}" class="w-full h-40 object-cover rounded-lg mb-4" loading="lazy" />
                <div class="route-info space-y-3">
                    <h3 class="text-xl font-bold text-white">${r.origin} → ${r.destination}</h3>
                    <p class="text-slate-400"><strong>Vehicle:</strong> ${r.vehicleType}</p>
                    <p class="text-slate-400"><strong>Departure:</strong> ${r.departureTime}</p>
                    <p class="text-slate-400"><strong>Seats Available:</strong> <span class="${seatColor} font-semibold">${r.availableSeats}</span></p>
                    ${statusBadge}
                    ${bookBtn}
                </div>
            </div>`;
        }).join('');

        attachBookBtnListeners();
    }

    // ── Booking Modal ──────────────────────────────────────────────────────
    const modal = document.getElementById('bookingModal');
    const successModal = document.getElementById('successModal');
    const closeModal = document.getElementById('closeModal');
    const closeSuccessModal = document.getElementById('closeSuccessModal');
    const confirmBtn = document.getElementById('confirmBookingBtn');
    const seatSelect = document.getElementById('seatCount');
    const bookingError = document.getElementById('bookingError');

    let activeRouteId = null;
    let activePricePerSeat = 0;

    function openModal(btn) {
        activeRouteId = parseInt(btn.dataset.routeId);
        const routeName = btn.dataset.route;
        activePricePerSeat = parseFloat(btn.dataset.price);
        const maxSeats = parseInt(btn.dataset.seats) || 10;

        document.getElementById('modalRouteName').textContent = routeName;
        document.getElementById('pricePerSeat').textContent = `PKR ${activePricePerSeat.toLocaleString()}`;
        document.getElementById('passengerName').value = '';
        document.getElementById('passengerEmail').value = '';

        // Build seat options up to available seats (max 10)
        seatSelect.innerHTML = Array.from({ length: Math.min(maxSeats, 10) }, (_, i) =>
            `<option value="${i + 1}">${i + 1} Seat${i > 0 ? 's' : ''}</option>`
        ).join('');

        updateTotal();
        bookingError.classList.add('hidden');
        modal.classList.remove('hidden');
        modal.classList.add('flex');
    }

    function updateTotal() {
        const seats = parseInt(seatSelect?.value || 1);
        const total = seats * activePricePerSeat;
        const el = document.getElementById('totalPriceDisplay');
        if (el) el.textContent = `PKR ${total.toLocaleString()}`;
    }

    if (seatSelect) seatSelect.addEventListener('change', updateTotal);

    if (closeModal) closeModal.addEventListener('click', () => {
        modal.classList.add('hidden');
        modal.classList.remove('flex');
    });

    if (closeSuccessModal) closeSuccessModal.addEventListener('click', () => {
        successModal.classList.add('hidden');
        successModal.classList.remove('flex');
    });

    // Close modals on backdrop click
    [modal, successModal].forEach(m => {
        m?.addEventListener('click', e => {
            if (e.target === m) { m.classList.add('hidden'); m.classList.remove('flex'); }
        });
    });

    if (confirmBtn) confirmBtn.addEventListener('click', async () => {
        const name = document.getElementById('passengerName').value.trim();
        const email = document.getElementById('passengerEmail').value.trim();
        const seats = parseInt(seatSelect.value);
        const routeName = document.getElementById('modalRouteName').textContent;

        // Validation
        bookingError.classList.add('hidden');
        if (!name) { showBookingError('Please enter your name.'); return; }
        if (!email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) { showBookingError('Please enter a valid email.'); return; }

        confirmBtn.disabled = true;
        confirmBtn.innerHTML = '<i class="fa-solid fa-spinner fa-spin mr-2"></i>Processing...';

        try {
            const res = await fetch('/Home/Book', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    routeId: activeRouteId,
                    route: routeName,
                    seats,
                    price: activePricePerSeat,
                    passengerName: name,
                    passengerEmail: email
                })
            });

            const data = await res.json();
            if (data.success) {
                modal.classList.add('hidden'); modal.classList.remove('flex');
                document.getElementById('successMessage').textContent = data.message;
                document.getElementById('bookingRefDisplay').textContent = data.bookingRef;
                successModal.classList.remove('hidden'); successModal.classList.add('flex');

                // Refresh page stats + route cards
                setTimeout(() => location.reload(), 5000);
            } else {
                showBookingError(data.message || 'Booking failed. Please try again.');
            }
        } catch {
            showBookingError('Network error. Please try again.');
        } finally {
            confirmBtn.disabled = false;
            confirmBtn.innerHTML = '<i class="fa-solid fa-check mr-2"></i>Confirm Booking';
        }
    });

    function showBookingError(msg) {
        bookingError.textContent = msg;
        bookingError.classList.remove('hidden');
    }

    function attachBookBtnListeners() {
        document.querySelectorAll('.book-btn').forEach(btn => {
            btn.addEventListener('click', () => openModal(btn));
        });
    }
    attachBookBtnListeners();

    // ── Live Tracking ──────────────────────────────────────────────────────
    const trackCards = document.querySelectorAll('.track-card');
    const trackLiveNote = document.getElementById('trackLiveNote');

    trackCards.forEach(card => {
        card.addEventListener('click', () => {
            trackCards.forEach(c => {
                c.classList.remove('bg-gradient-to-r', 'from-cyan-500/20', 'to-blue-500/20', 'border-cyan-500/50', 'active-tracking');
                c.classList.add('bg-slate-800/50', 'border-slate-700/50');
            });
            card.classList.add('bg-gradient-to-r', 'from-cyan-500/20', 'to-blue-500/20', 'border-cyan-500/50');
            if (trackLiveNote) {
                trackLiveNote.textContent = `Tracking: ${card.dataset.route} — ETA ${card.dataset.eta}`;
            }
        });
    });

    // Animate progress bars on scroll
    const observer = new IntersectionObserver(entries => {
        entries.forEach(e => {
            if (e.isIntersecting) {
                e.target.querySelectorAll('.track-progress-fill').forEach(bar => {
                    bar.style.transition = 'width 1s ease';
                });
            }
        });
    }, { threshold: 0.2 });
    document.querySelector('.track-list')?.let && observer.observe(document.querySelector('.track-list'));
    if (document.querySelector('.track-list')) observer.observe(document.querySelector('.track-list'));

    // ── Booking Tracker ────────────────────────────────────────────────────
    const trackBtn = document.getElementById('trackBookingBtn');
    const refInput = document.getElementById('bookingRefInput');
    const bookingResult = document.getElementById('bookingResult');

    if (trackBtn) {
        trackBtn.addEventListener('click', () => fetchBooking());
        refInput?.addEventListener('keydown', e => { if (e.key === 'Enter') fetchBooking(); });
    }

    async function fetchBooking() {
        const ref = refInput?.value.trim();
        if (!ref) { showBookingResult(false, null, 'Please enter a booking reference.'); return; }

        trackBtn.disabled = true;
        trackBtn.innerHTML = '<i class="fa-solid fa-spinner fa-spin mr-2"></i>Searching...';

        try {
            const res = await fetch(`/Home/TrackBooking?ref=${encodeURIComponent(ref)}`);
            const data = await res.json();
            showBookingResult(data.success, data.success ? data : null, data.success ? null : data.message);
        } catch {
            showBookingResult(false, null, 'Network error. Please try again.');
        } finally {
            trackBtn.disabled = false;
            trackBtn.innerHTML = '<i class="fa-solid fa-search mr-2"></i>Track';
        }
    }

    function showBookingResult(success, data, errorMsg) {
        if (!bookingResult) return;
        bookingResult.classList.remove('hidden');

        if (!success) {
            bookingResult.innerHTML = `
                <div class="p-4 rounded-lg bg-red-500/20 border border-red-500/30 text-red-300 text-center">
                    <i class="fa-solid fa-circle-xmark mr-2"></i>${errorMsg}
                </div>`;
            return;
        }

        const statusColor = data.status === 'Confirmed' ? 'text-green-400' : 'text-red-400';
        const statusIcon = data.status === 'Confirmed' ? 'fa-circle-check text-green-400' : 'fa-circle-xmark text-red-400';

        bookingResult.innerHTML = `
            <div class="p-6 rounded-xl bg-slate-800/50 border border-cyan-500/30">
                <div class="flex justify-between items-start mb-4">
                    <div>
                        <p class="text-slate-400 text-sm">Booking Reference</p>
                        <p class="text-2xl font-bold text-cyan-400 tracking-wider">${data.bookingRef}</p>
                    </div>
                    <span class="${statusColor} font-semibold flex items-center gap-2">
                        <i class="fa-solid ${statusIcon}"></i> ${data.status}
                    </span>
                </div>
                <div class="grid grid-cols-2 gap-4 text-sm mb-6">
                    <div><p class="text-slate-400">Route</p><p class="text-white font-semibold">${data.route}</p></div>
                    <div><p class="text-slate-400">Vehicle</p><p class="text-white font-semibold">${data.vehicleType}</p></div>
                    <div><p class="text-slate-400">Departure</p><p class="text-white font-semibold">${data.departureTime}</p></div>
                    <div><p class="text-slate-400">Passenger</p><p class="text-white font-semibold">${data.passengerName}</p></div>
                    <div><p class="text-slate-400">Seats</p><p class="text-white font-semibold">${data.seats}</p></div>
                    <div><p class="text-slate-400">Total Paid</p><p class="text-cyan-400 font-bold">PKR ${parseFloat(data.totalPrice).toLocaleString()}</p></div>
                    <div class="col-span-2"><p class="text-slate-400">Booked At</p><p class="text-white">${data.bookedAt}</p></div>
                </div>
                ${data.status === 'Confirmed' ? `
                <button id="cancelBookingBtn" data-ref="${data.bookingRef}"
                    class="w-full px-4 py-2 bg-red-500/20 hover:bg-red-500/40 border border-red-500/30 text-red-300 font-semibold rounded-lg smooth-transition">
                    <i class="fa-solid fa-ban mr-2"></i>Cancel Booking
                </button>` : ''}
            </div>`;

        document.getElementById('cancelBookingBtn')?.addEventListener('click', async function () {
            if (!confirm('Are you sure you want to cancel this booking?')) return;
            try {
                const r = await fetch('/Home/CancelBooking', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ bookingRef: this.dataset.ref })
                });
                const d = await r.json();
                if (d.success) {
                    alert('Booking cancelled successfully.');
                    fetchBooking();
                } else {
                    alert(d.message);
                }
            } catch { alert('Error cancelling booking.'); }
        });
    }

    // ── Smooth Scroll ──────────────────────────────────────────────────────
    document.querySelectorAll('a[href^="#"]').forEach(link => {
        link.addEventListener('click', e => {
            const target = document.querySelector(link.getAttribute('href'));
            if (target) { e.preventDefault(); target.scrollIntoView({ behavior: 'smooth' }); }
        });
    });

});
