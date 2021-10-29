import torch
import torch.nn.functional as F
from torch import nn
from torch.autograd import Variable
import scipy.io as io
import numpy as np
import matplotlib.pyplot as plt

def get_acc(output, label):
    total = output.shape[0]
    _, pred_label = output.max(1)
    num_correct= (pred_label == label).sum().data
    return float(num_correct) / total

def train(net, train_data, valid_data, num_epochs, optimizer, criterion):
    if torch.cuda.is_available():
        net = net.cuda()
    for epoch in range(num_epochs):
        # train part
        train_loss = 0.0
        train_acc = 0.0
        for im, label in train_data:
            if torch.cuda.is_available():
                with torch.no_grad():
                    im = Variable(im.cuda())
                    label = Variable(label.cuda())
            else:
                with torch.no_grad():
                    im = Variable(im)
                    label = Variable(label)
            # forward
            output, _ = net(im)
            loss = criterion(output, label)
            # backward
            optimizer.zero_grad()
            loss.backward()
            optimizer.step()

            train_loss += float(loss.data)
            train_acc += float(get_acc(output, label))
        # valid part

        dic = dict()

        if valid_data is not None:
            valid_loss = 0.0
            valid_acc = 0.0
            net = net.eval()
            for im, label in valid_data:
                if torch.cuda.is_available():
                    with torch.no_grad():
                        im = Variable(im.cuda())
                        label = Variable(label.cuda())
                else:
                    with torch.no_grad():
                        im = Variable(im)
                        label = Variable(label)
                output, _ = net(im)
                loss = criterion(output, label)
                valid_loss += float(loss.data)
                valid_acc += float(get_acc(output, label))

            savedString = (
                "Epoch %d.\nTrain Loss: %f.\nTrain Acc: %f.\nValid Loss: %f.\nValid Acc: %f.\n"
                % (epoch, train_loss / len(train_data),
                    train_acc / len(train_data), valid_loss / len(valid_data),
                    valid_acc / len(valid_data)))

            try:
                with open('../CNNV/Assets/Resources/Textures/CIFAR10/cifar10_matrix_data.txt', 'w') as f:
                    f.write(savedString)
            except:
                pass
            

            epoch_str = (
                "Epoch %d. Train Loss: %f, Train Acc: %f, Valid Loss: %f, Valid Acc: %f, "
                % (epoch, train_loss / len(train_data),
                    train_acc / len(train_data), valid_loss / len(valid_data),
                    valid_acc / len(valid_data)))
            dic
        else:
            epoch_str = ("Epoch %d. Train Loss: %f, Train Acc: %f, " %
                (epoch, train_loss / len(train_data),
                train_acc / len(train_data)))
        print(epoch_str)

        

        for im, data in valid_data:
            if torch.cuda.is_available():
                with torch.no_grad():
                    im = Variable(im.cuda())
                    label = Variable(label.cuda())
            with torch.no_grad():
                im = Variable(im, volatile=True)[0]
                dic['image_R'] = im.squeeze()[0].detach().cpu().numpy().copy()
                dic['image_G'] = im.squeeze()[1].detach().cpu().numpy().copy()
                dic['image_B'] = im.squeeze()[2].detach().cpu().numpy().copy()
            im = im.unsqueeze(0)
            output, res = net(im)
            dic['output'] = output.argmax().detach().cpu().numpy().copy()
            for i in range(len(res)):
                res[i] = res[i][0].detach().cpu().numpy()
                for j in range(res[i].shape[0]):
                    dic['{:03d}_{:03d}'.format(i, j)] = res[i][j].copy()
            try:
                with open('../CNNV/Assets/Resources/Textures/CIFAR10/cifar10_matrix_data.mat', 'wb') as f:
                    io.savemat(f, dic)
            except:
                pass
            break