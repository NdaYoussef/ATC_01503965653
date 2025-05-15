// Initialize localStorage for users and events if not present
if (!localStorage.getItem('users')) {
    localStorage.setItem('users', JSON.stringify([]));
}
if (!localStorage.getItem('events')) {
    localStorage.setItem('events', JSON.stringify([
        {
            id: 1,
            name: 'Sample Event',
            description: 'Description of the event.',
            category: 'Music',
            date: '2023-10-01',
            venue: 'Main Hall',
            price: 20,
            image: 'https://placehold.co/300x200',
            booked: true
        },
        {
            id: 2,
            name: 'Another Event',
            description: 'Description of the event.',
            category: 'Concert',
            date: '2023-11-01',
            venue: 'City Arena',
            price: 30,
            image: 'https://placehold.co/300x200',
            booked: false
        }
    ]));
}

let currentUser = null;

function showHome() {
    hideAllSections();
    document.getElementById('home-section').classList.remove('hidden');
    renderEvents();
}

function showLogin() {
    hideAllSections();
    document.getElementById('login-section').classList.remove('hidden');
}

function showRegister() {
    hideAllSections();
    document.getElementById('register-section').classList.remove('hidden');
}

function showEventDetails(eventId) {
    hideAllSections();
    document.getElementById('event-details').classList.remove('hidden');
    const events = JSON.parse(localStorage.getItem('events'));
    const event = events.find(e => e.id === eventId);
    document.getElementById('event-details-content').innerHTML = `
        <h3 class="text-xl font-semibold mb-2">${event.name}</h3>
        <p class="mb-2"><strong>Description:</strong> ${event.description}</p>
        <p class="mb-2"><strong>Category:</strong> ${event.category}</p>
        <p class="mb-2"><strong>Date:</strong> ${event.date}</p>
        <p class="mb-2"><strong>Venue:</strong> ${event.venue}</p>
        <p class="mb-4"><strong>Price:</strong> $${event.price}</p>
        <img src="${event.image}" alt="Event Image" class="w-full rounded-lg mb-4 object-cover h-48" />
        <button class="bg-secondary text-secondary-foreground hover:bg-secondary/90 p-3 rounded w-full transition duration-200" onclick="bookEvent(${event.id})">
            ${event.booked ? 'Booked' : 'Book Now'}
        </button>
    `;
}

function hideAllSections() {
    document.getElementById('home-section').classList.add('hidden');
    document.getElementById('login-section').classList.add('hidden');
    document.getElementById('register-section').classList.add('hidden');
    document.getElementById('admin-dashboard').classList.add('hidden');
    document.getElementById('event-details').classList.add('hidden');
}

function handleRegister() {
    const username = document.getElementById('register-username').value;
    const email = document.getElementById('register-email').value;
    const password = document.getElementById('register-password').value;

    if (!username || !email || !password) {
        alert('Please fill in all fields.');
        return;
    }

    const users = JSON.parse(localStorage.getItem('users'));
    if (users.find(user => user.username === username)) {
        alert('Username already exists.');
        return;
    }

    users.push({ username, email, password, isAdmin: username === 'admin' });
    localStorage.setItem('users', JSON.stringify(users));
    alert('Registration successful! Please log in.');
    showLogin();
}

function handleLogin() {
    const username = document.getElementById('login-username').value;
    const password = document.getElementById('login-password').value;

    const users = JSON.parse(localStorage.getItem('users'));
    const user = users.find(u => u.username === username && u.password === password);

    if (!user) {
        alert('Invalid username or password.');
        return;
    }

    currentUser = user;
    hideAllSections();

    if (user.isAdmin) {
        document.getElementById('admin-dashboard').classList.remove('hidden');
        document.getElementById('nav-menu').innerHTML = `
            <li><a href="#" onclick="showHome()" class="block hover:bg-primary/80 px-4 py-2 rounded transition duration-200">Home</a></li>
            <li><a href="#" onclick="showAdminDashboard()" class="block hover:bg-primary/80 px-4 py-2 rounded transition duration-200">Manage Events</a></li>
            <li><a href="#" onclick="handleLogout()" class="block hover:bg-primary/80 px-4 py-2 rounded transition duration-200">Logout</a></li>
        `;
    } else {
        showHome();
        document.getElementById('nav-menu').innerHTML = `
            <li><a href="#" onclick="showHome()" class="block hover:bg-primary/80 px-4 py-2 rounded transition duration-200">Home</a></li>
            <li><a href="#" onclick="handleLogout()" class="block hover:bg-primary/80 px-4 py-2 rounded transition duration-200">Logout</a></li>
        `;
    }
}

function showAdminDashboard() {
    hideAllSections();
    document.getElementById('admin-dashboard').classList.remove('hidden');
    document.getElementById('create-event-form').classList.add('hidden');
    document.getElementById('view-events-list').classList.add('hidden');
}

function handleLogout() {
    currentUser = null;
    hideAllSections();
    showHome();
    document.getElementById('nav-menu').innerHTML = `
        <li><a href="#" onclick="showHome()" class="block hover:bg-primary/80 px-4 py-2 rounded transition duration-200">Home</a></li>
        <li><a href="#" onclick="showRegister()" class="block hover:bg-primary/80 px-4 py-2 rounded transition duration-200">Register</a></li>
        <li><a href="#" onclick="showLogin()" class="block hover:bg-primary/80 px-4 py-2 rounded transition duration-200">Login</a></li>
    `;
}

function renderEvents() {
    const eventList = document.getElementById('event-list');
    const events = JSON.parse(localStorage.getItem('events')) || [];
    eventList.innerHTML = events.length ? events.map(event => `
        <div class="border border-border rounded-lg p-5 shadow-lg hover:shadow-xl transition duration-300">
            <img src="${event.image}" alt="Event Image" class="w-full rounded-lg mb-4 object-cover h-48" />
            <h3 class="text-xl font-semibold">${event.name}</h3>
            <p class="text-muted-foreground mb-4">${event.description}</p>
            <div class="flex gap-2">
                <button class="bg-secondary text-secondary-foreground hover:bg-secondary/90 p-3 rounded flex-1 transition duration-200" onclick="showEventDetails(${event.id})">View Details</button>
                <button class="bg-green-500 text-white hover:bg-green-600 p-3 rounded flex-1 transition duration-200" onclick="bookEvent(${event.id})">
                    ${event.booked ? 'Booked' : 'Book Now'}
                </button>
            </div>
        </div>
    `).join('') : '<p class="text-muted-foreground">No events available.</p>';
}

function bookEvent(eventId) {
    if (!currentUser) {
        alert('Please log in to book an event.');
        showLogin();
        return;
    }
    const events = JSON.parse(localStorage.getItem('events'));
    const event = events.find(e => e.id === eventId);
    event.booked = true;
    localStorage.setItem('events', JSON.stringify(events));
    renderEvents();
    if (document.getElementById('event-details').classList.contains('hidden') === false) {
        showEventDetails(eventId);
    }
}

function showCreateEventForm() {
    document.getElementById('create-event-form').classList.remove('hidden');
    document.getElementById('view-events-list').classList.add('hidden');
    document.getElementById('event-name').value = '';
    document.getElementById('event-description').value = '';
    document.getElementById('event-category').value = '';
    document.getElementById('event-date').value = '';
    document.getElementById('event-venue').value = '';
    document.getElementById('event-price').value = '';
}

function createEvent() {
    const name = document.getElementById('event-name').value;
    const description = document.getElementById('event-description').value;
    const category = document.getElementById('event-category').value;
    const date = document.getElementById('event-date').value;
    const venue = document.getElementById('event-venue').value;
    const price = parseFloat(document.getElementById('event-price').value);

    if (!name || !description || !category || !date || !venue || !price) {
        alert('Please fill in all fields.');
        return;
    }

    const events = JSON.parse(localStorage.getItem('events')) || [];
    const newEvent = {
        id: events.length ? Math.max(...events.map(e => e.id)) + 1 : 1,
        name,
        description,
        category,
        date,
        venue,
        price,
        image: 'https://placehold.co/300x200',
        booked: false
    };
    events.push(newEvent);
    localStorage.setItem('events', JSON.stringify(events));
    alert('Event created successfully!');
    showViewEvents();
}

function showViewEvents() {
    document.getElementById('create-event-form').classList.add('hidden');
    document.getElementById('view-events-list').classList.remove('hidden');
    const adminEventList = document.getElementById('admin-event-list');
    const events = JSON.parse(localStorage.getItem('events')) || [];
    adminEventList.innerHTML = events.length ? events.map(event => `
        <div class="border border-border rounded-lg p-4 flex justify-between items-center">
            <div>
                <h5 class="text-lg font-semibold">${event.name}</h5>
                <p class="text-muted-foreground">${event.date} | ${event.venue}</p>
            </div>
            <div class="flex gap-2">
                <button class="bg-muted text-muted-foreground hover:bg-muted/90 p-2 rounded" onclick="showUpdateEventForm(${event.id})">Edit</button>
                <button class="bg-destructive text-destructive-foreground hover:bg-destructive Manila/90 p-2 rounded" onclick="deleteEvent(${event.id})">Delete</button>
            </div>
        </div>
    `).join('') : '<p class="text-muted-foreground">No events available.</p>';
}

function showUpdateEventForm(eventId) {
    const events = JSON.parse(localStorage.getItem('events'));
    const event = events.find(e => e.id === eventId);
    document.getElementById('create-event-form').classList.remove('hidden');
    document.getElementById('view-events-list').classList.add('hidden');
    document.getElementById('event-name').value = event.name;
    document.getElementById('event-description').value = event.description;
    document.getElementById('event-category').value = event.category;
    document.getElementById('event-date').value = event.date;
    document.getElementById('event-venue').value = event.venue;
    document.getElementById('event-price').value = event.price;
    document.getElementById('create-event-form').innerHTML += `
        <button id="update-event-btn" class="bg-primary text-primary-foreground hover:bg-primary/90 p-3 rounded w-full transition duration-200 mt-4" onclick="updateEvent(${event.id})">Update Event</button>
    `;
    document.querySelector('#create-event-form button[onclick^="createEvent"]').style.display = 'none';
}

function updateEvent(eventId) {
    const name = document.getElementById('event-name').value;
    const description = document.getElementById('event-description').value;
    const category = document.getElementById('event-category').value;
    const date = document.getElementById('event-date').value;
    const venue = document.getElementById('event-venue').value;
    const price = parseFloat(document.getElementById('event-price').value);

    if (!name || !description || !category || !date || !venue || !price) {
        alert('Please fill in all fields.');
        return;
    }

    const events = JSON.parse(localStorage.getItem('events'));
    const eventIndex = events.findIndex(e => e.id === eventId);
    events[eventIndex] = {
        ...events[eventIndex],
        name,
        description,
        category,
        date,
        venue,
        price
    };
    localStorage.setItem('events', JSON.stringify(events));
    alert('Event updated successfully!');
    showViewEvents();
}

function deleteEvent(eventId) {
    if (confirm('Are you sure you want to delete this event?')) {
        const events = JSON.parse(localStorage.getItem('events'));
        const updatedEvents = events.filter(e => e.id !== eventId);
        localStorage.setItem('events', JSON.stringify(updatedEvents));
        showViewEvents();
        renderEvents();
    }
}

// Initialize page
showHome();