# Electric Vehicle Dealer Management System

A comprehensive, production-ready React.js application for managing electric vehicle dealerships and manufacturer operations. Built with modern technologies and featuring a clean, responsive UI.

## 🚀 Features

### Dealer Portal
- **Vehicle Information Management**
  - Browse vehicle catalog with filtering and search
  - Compare models and features
  - View detailed specifications and pricing

- **Sales Management**
  - Create and manage quotations
  - Process sales orders and contracts
  - Track vehicle delivery status
  - Manage payment status (full payment, installment)
  - Handle promotions and discount campaigns

- **Customer Management**
  - Store and manage customer profiles
  - Schedule and track test drives
  - Collect feedback and handle complaints

- **Reporting & Analytics**
  - Sales performance by salesperson
  - Customer debt tracking
  - Manufacturer/dealer reports

### Manufacturer Portal
- **Product & Distribution Management**
  - Manage vehicle catalog (models, versions, colors)
  - Inventory management and allocation to dealers
  - Pricing and dealer-specific discount policies

- **Dealer Management**
  - Manage dealer profiles and sales targets
  - Track dealer debts and performance
  - Manage dealer accounts on the system

- **Reports & Analytics**
  - Regional sales reports by dealer
  - Stock levels and consumption rates
  - AI-based demand forecasting for production planning

## 🛠️ Technology Stack

- **Frontend Framework**: React 18 with TypeScript
- **Build Tool**: Vite
- **UI Library**: Material-UI v5 (MUI)
- **State Management**: Redux Toolkit
- **Data Fetching**: React Query (TanStack Query)
- **Routing**: React Router v6
- **Charts**: Recharts
- **Data Grid**: MUI X Data Grid

## 📁 Project Structure

```
src/
├── components/           # Reusable UI components
│   ├── auth/            # Authentication components
│   ├── charts/          # Chart components
│   ├── common/          # Common UI components
│   ├── forms/           # Form components
│   └── layout/          # Layout components
├── hooks/               # Custom React hooks
├── pages/               # Page components
│   ├── dealer/          # Dealer portal pages
│   └── manufacturer/    # Manufacturer portal pages
├── services/            # API services and mock data
├── store/               # Redux store and slices
├── types/               # TypeScript type definitions
└── App.tsx              # Main application component
```

## 🚦 Getting Started

### Prerequisites

- Node.js (v16 or higher)
- npm or yarn

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ev-dealer-management
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Start the development server**
   ```bash
   npm run dev
   ```

4. **Open your browser**
   Navigate to `http://localhost:3000`

### Build for Production

```bash
npm run build
```

## 🔐 Authentication & User Roles

The system supports four user roles with different access levels:

### Demo Credentials

| Role | Email | Password | Access Level |
|------|-------|----------|--------------|
| Admin | admin@evm.com | password | Full system access |
| EVM Staff | evm.staff@evm.com | password | Manufacturer portal |
| Dealer Manager | manager@dealer1.com | password | Dealer portal (full access) |
| Dealer Staff | staff@dealer1.com | password | Dealer portal (limited access) |

### Role-Based Access Control

- **Admin**: Full system access, can manage all dealers and users
- **EVM Staff**: Manufacturer portal access, can manage products and dealers
- **Dealer Manager**: Full dealer portal access, can view reports and manage staff
- **Dealer Staff**: Limited dealer portal access, can manage customers and sales

## 📊 Key Features

### Dashboard Analytics
- Real-time sales performance metrics
- Interactive charts and visualizations
- Key performance indicators (KPIs)
- Trend analysis and growth metrics

### Vehicle Management
- Comprehensive vehicle catalog
- Advanced filtering and search capabilities
- Vehicle comparison tools
- Inventory tracking and management

### Sales Process
- End-to-end sales workflow
- Quotation generation and management
- Order processing and tracking
- Contract management and execution

### Customer Relationship Management
- Customer profile management
- Test drive scheduling
- Feedback collection and analysis
- Communication tracking

## 🎨 UI/UX Features

- **Responsive Design**: Works seamlessly on desktop, tablet, and mobile
- **Modern Material Design**: Clean, intuitive interface following Material Design principles
- **Dark/Light Theme**: Theme switching capability
- **Accessibility**: WCAG compliant with keyboard navigation support
- **Performance**: Optimized for fast loading and smooth interactions

## 🔧 Development

### Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint

### Code Quality

- TypeScript for type safety
- ESLint for code linting
- Prettier for code formatting
- Component-based architecture
- Custom hooks for reusable logic

## 📱 Responsive Design

The application is fully responsive and optimized for:
- Desktop (1200px+)
- Tablet (768px - 1199px)
- Mobile (320px - 767px)

## 🔒 Security Features

- Role-based access control
- Protected routes
- Input validation and sanitization
- Secure authentication flow
- CSRF protection ready

## 🚀 Deployment

The application can be deployed to any static hosting service:

- **Vercel**: `vercel --prod`
- **Netlify**: Connect your repository
- **AWS S3**: Upload the build folder
- **GitHub Pages**: Use GitHub Actions

## 📈 Performance

- **Bundle Size**: Optimized with Vite
- **Code Splitting**: Automatic route-based splitting
- **Lazy Loading**: Components loaded on demand
- **Caching**: React Query for efficient data caching

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Check the documentation
- Review the code comments

## 🔮 Future Enhancements

- Real-time notifications
- Advanced analytics and AI insights
- Mobile app development
- Integration with external APIs
- Multi-language support
- Advanced reporting features

---

**Built with ❤️ using React, TypeScript, and Material-UI**
