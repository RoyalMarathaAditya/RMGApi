import toast from 'react-hot-toast';

export const toastService = {
  success: (message: string) =>
    toast.success(message, { duration: 4000 }),

  error: (message: string) =>
    toast.error(message, { duration: 5000 }),

  warning: (message: string) =>
    toast(message, {
      duration: 4000,
      icon: '⚠',
      style: { background: '#EA580C', color: '#fff' },
    }),

  info: (message: string) =>
    toast(message, {
      duration: 4000,
      icon: 'ℹ',
      style: { background: '#2563EB', color: '#fff' },
    }),
};
